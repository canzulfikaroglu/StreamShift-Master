﻿using StreamShift.Domain.DatabaseEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StreamShift.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Driver;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using StreamShift.Infrastructure.Helpers.DynamicQueryHelper;
using Microsoft.Identity.Client;

namespace StreamShift.Infrastructure.Extension
{
    public static class QueryExtensions
    {
        public static string GetSchemaQuery(this eDatabase database)
        {
            return database switch
            {
                eDatabase.MsSqlServer => @"SELECT TABLE_SCHEMA AS SchemaName, TABLE_NAME AS TableName, COLUMN_NAME AS ColumnName, DATA_TYPE AS DataType, CHARACTER_MAXIMUM_LENGTH AS MaxLength FROM INFORMATION_SCHEMA.COLUMNS ORDER BY TABLE_SCHEMA, TABLE_NAME, ORDINAL_POSITION;",
                eDatabase.Postgres => @"SELECT c.table_schema AS SchemaName, c.table_name AS TableName, c.column_name AS ColumnName, c.data_type AS DataType, c.character_maximum_length AS MaxLength, CASE WHEN pk.column_name IS NOT NULL THEN 'YES' ELSE 'NO' END AS IsPrimaryKey, CASE WHEN c.is_nullable = 'NO' THEN 'YES' ELSE 'NO' END AS IsNotNull, pg_get_expr(ad.adbin, ad.adrelid) AS DefaultValue FROM information_schema.columns c LEFT JOIN ( SELECT tc.table_schema, tc.table_name, kcu.column_name FROM information_schema.table_constraints tc JOIN information_schema.key_column_usage kcu ON tc.constraint_name = kcu.constraint_name AND tc.table_schema = kcu.table_schema WHERE tc.constraint_type = 'PRIMARY KEY' ) pk ON c.table_schema = pk.table_schema AND c.table_name = pk.table_name AND c.column_name = pk.column_name LEFT JOIN pg_attrdef ad ON ad.adrelid = ( SELECT oid FROM pg_class WHERE relname = c.table_name AND relnamespace = ( SELECT oid FROM pg_namespace WHERE nspname = c.table_schema ) ) AND ad.adnum = ( SELECT ordinal_position FROM information_schema.columns WHERE table_schema = c.table_schema AND table_name = c.table_name AND column_name = c.column_name ) WHERE c.table_schema NOT IN ('pg_catalog', 'information_schema') ORDER BY c.table_schema, c.table_name, c.ordinal_position;",
                eDatabase.Sqlite => @"SELECT name AS TableName FROM sqlite_master WHERE type='table' ORDER BY name;",
                _ => throw new NotSupportedException($"The database type {database} is not supported.")
            };
        }
        public static string GetCreateTableQuery(this eDatabase database, string tableName, IEnumerable<TableSchema> columns)
        {
            if (string.IsNullOrWhiteSpace(tableName) || columns == null || !columns.Any())
                throw new ArgumentException("Table name and columns must be provided.");
            var columnDefinitions = columns.Select(c =>
            {
                var columnDef = $"{c.ColumnName} {c.DataType}";
                // Veri tipi VARCHAR ise uzunluk ekle
                if (c.DataType.ToLower().Contains("character varying") && c.MaxLength.HasValue)
                {
                    columnDef += $"({c.MaxLength})";
                }
                // NOT NULL ekle
                if (c.IsNotNull == "YES")
                {
                    columnDef += " NOT NULL";
                }
                // Varsayılan değer ekle
                if (!string.IsNullOrWhiteSpace(c.DefaultValue))
                {
                    columnDef += $" DEFAULT {c.DefaultValue}";
                }
                return columnDef;
            });
            // Primary Key tanımlamaları
            var primaryKeys = columns
                .Where(c => c.IsPrimaryKey == "YES" ? true : false)
                .Select(c => c.ColumnName)
                .ToList();
            string primaryKeyConstraint = primaryKeys.Any()
                ? $", PRIMARY KEY ({string.Join(", ", primaryKeys)})"
                : string.Empty;
          //  TableSequenceCreate(tableName, columnDef, primaryKeys);
            // Tüm SQL sorgusunu birleştirme
            string createTableQuery = database switch
            {
                eDatabase.MsSqlServer => $@"CREATE TABLE {tableName} ({string.Join(", ", columnDefinitions)} {primaryKeyConstraint});",

                eDatabase.Postgres => $@"CREATE TABLE {tableName} ({string.Join(", ", columnDefinitions)} {primaryKeyConstraint});",

                eDatabase.Sqlite => $@"CREATE TABLE {tableName} ({string.Join(", ", columnDefinitions)} {primaryKeyConstraint});",

                _ => throw new NotSupportedException($"The database type {database} is not supported.")
            };
            return createTableQuery;
        }
        public static string TableExistsQuery(this eDatabase database, string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("Table name must be provided.");
            string query = database switch
            {
                eDatabase.MsSqlServer => $@"SELECT CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}') THEN 1 ELSE 0 END;",

                eDatabase.Postgres => $"SELECT EXISTS ( SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName}' ) AS Exist",

                eDatabase.Sqlite => $@"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'",

                _ => throw new NotSupportedException($"The database type {database} is not supported.")
            };
            return query;
        }
        public static void TableSequenceCreate(List<TableSchema> sourceSchema,DbContext _destinationDbContext,eDatabase destinationDatabase)
        {
            if (destinationDatabase == eDatabase.Postgres) {
                foreach (var schema in sourceSchema)
                {
                    if (schema.IsPrimaryKey == "YES")
                    {
                        string sequenceSql = $@"CREATE SEQUENCE {schema.TableName}_{schema.ColumnName}_seq";
                        var createSequence = _destinationDbContext.Database.ExecuteSqlRaw(sequenceSql);
                    }
                }

            }
           
        }
        //public static string GetInsertQuery(this eDatabase database, string tableName, IEnumerable<TableSchema> columns,List<dynamic> row)
        //{
        //        var columnNames = string.Join(", ", columns.Select(c => c.ColumnName));
        //        var values = string.Join(", ", row.Select(value => $"'{value}'")); // Veriyi uygun şekilde formatlayın

        //    return database switch
        //    {
        //        eDatabase.MsSqlServer => $"INSERT INTO {tableName} ({columnNames}) VALUES ({values});",
        //        eDatabase.Postgres => $"INSERT INTO {tableName} ({columnNames}) VALUES ({values});",
        //        eDatabase.Sqlite => $"INSERT INTO {tableName} ({columnNames}) VALUES ({values});",
        //        _ => throw new NotSupportedException($"The database type {database} is not supported.")
        //    };
        //}
        public static void GetSelectQuery(string TableName, DbContext _sourceDbContext) //tablodaki verileri select sorgusu ile çekme fonksiyonu
        {

            string SelectQuery = $@"SELECT * FROM {TableName}";
            var ExecuteQuery = _sourceDbContext.Database.ExecuteSqlRaw(SelectQuery);
            Console.WriteLine(ExecuteQuery);
        }
        public static string TableQuotationMark(object key,object value,List<dynamic> rows,string tableName,out string topluColumn,out string topluData)
        {
            string columnName = "";
            string data = "";
            topluData = "";// dışarıya gönderdiğimiz değerler
            topluColumn = "";// dışarıya gönderdiğimiz değerler
            columnName = key.ToString();
            data = value.ToString();
            data = "'" + data + "'";
            topluData = data;
            topluColumn = columnName;
            return "CANBABA3ve1";
        }
    }
}