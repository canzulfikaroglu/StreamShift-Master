using StreamShift.Domain.DatabaseEnums;
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
                eDatabase.MsSqlServer => @"SELECT c.TABLE_SCHEMA AS schemaname, c.TABLE_NAME AS tablename, c.COLUMN_NAME AS columnname, c.DATA_TYPE AS datatype, c.CHARACTER_MAXIMUM_LENGTH AS maxlength, CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'YES' ELSE 'NO' END AS isprimarykey, CASE WHEN c.IS_NULLABLE = 'NO' THEN 'YES' ELSE 'NO' END AS isnotnull, CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'YES' ELSE c.COLUMN_DEFAULT END AS defaultvalue FROM INFORMATION_SCHEMA.COLUMNS c LEFT JOIN (SELECT tc.TABLE_SCHEMA, tc.TABLE_NAME, kcu.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu ON tc.CONSTRAINT_NAME = kcu.CONSTRAINT_NAME AND tc.TABLE_SCHEMA = kcu.TABLE_SCHEMA WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY') pk ON c.TABLE_SCHEMA = pk.TABLE_SCHEMA AND c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME WHERE c.TABLE_SCHEMA NOT IN ('INFORMATION_SCHEMA', 'sys') ORDER BY c.TABLE_SCHEMA, c.TABLE_NAME, c.ORDINAL_POSITION;",                 
                eDatabase.Postgres => @"SELECT c.table_schema AS SchemaName, c.table_name AS TableName, c.column_name AS ColumnName, c.data_type AS DataType, c.character_maximum_length AS MaxLength, CASE WHEN pk.column_name IS NOT NULL THEN 'YES' ELSE 'NO' END AS IsPrimaryKey, CASE WHEN c.is_nullable = 'NO' THEN 'YES' ELSE 'NO' END AS IsNotNull, pg_get_expr(ad.adbin, ad.adrelid) AS DefaultValue FROM information_schema.columns c LEFT JOIN ( SELECT tc.table_schema, tc.table_name, kcu.column_name FROM information_schema.table_constraints tc JOIN information_schema.key_column_usage kcu ON tc.constraint_name = kcu.constraint_name AND tc.table_schema = kcu.table_schema WHERE tc.constraint_type = 'PRIMARY KEY' ) pk ON c.table_schema = pk.table_schema AND c.table_name = pk.table_name AND c.column_name = pk.column_name LEFT JOIN pg_attrdef ad ON ad.adrelid = ( SELECT oid FROM pg_class WHERE relname = c.table_name AND relnamespace = ( SELECT oid FROM pg_namespace WHERE nspname = c.table_schema ) ) AND ad.adnum = ( SELECT ordinal_position FROM information_schema.columns WHERE table_schema = c.table_schema AND table_name = c.table_name AND column_name = c.column_name ) WHERE c.table_schema NOT IN ('pg_catalog', 'information_schema') ORDER BY c.table_schema, c.table_name, c.ordinal_position;",
                eDatabase.Sqlite => @"SELECT name AS TableName FROM sqlite_master WHERE type='table' ORDER BY name;",
                _ => throw new NotSupportedException($"The database type {database} is not supported.")
            };
        }
        public static string GetCreateTableQuery(this eDatabase database, string tableName, IEnumerable<TableSchema> columns,out List<string> primaryKeys)
        {/*mssql sorgu CREATE TABLE kategoriler (kategori_id INT NOT NULL IDENTITY(1,1),kategori_adi VARCHAR(50) NOT NULL,PRIMARY KEY (kategori_id));
          
       postgresql   CREATE TABLE kategoriler (kategori_id integer NOT NULL DEFAULT nextval('kategoriler_kategori_id_seq'::regclass), kategori_adi character varying(50) NOT NULL , PRIMARY KEY (kategori_id));
                    CREATE TABLE kategoriler (kategori_id integer NOT NULL DEFAULT nextval('kategoriler_kategori_id_seq'::regclass), kategori_adi character varying(50) NOT NULL , PRIMARY KEY (kategori_id));
          */

            if (string.IsNullOrWhiteSpace(tableName) || columns == null || !columns.Any())
                throw new ArgumentException("Table name and columns must be provided.");

            var columnDefinitions = columns.Select(c =>
            {
                var columnDef = "";
                if (c.DataType.ToLower().Contains("bigint") || c.DataType.ToLower().Contains("numeric")) {
                    columnDef = $"{c.ColumnName} {c.DataType}";
                }
                
                if(database == eDatabase.MsSqlServer && c.DataType== "character varying")
                {
                    columnDef += $"{c.ColumnName} VARCHAR";
                }
                if (database == eDatabase.Postgres)// postgresql'e göre 
                {
                    if (c.DataType.ToLower().Contains("varchar") && c.MaxLength!=null)
                    {
                        columnDef += $"{c.ColumnName} character varying({c.MaxLength})";
                    }
                    if (c.DataType=="int")
                    {
                        columnDef += $"{c.ColumnName} integer";
                    }
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
                        columnDef += $" DEFAULT nextval('{c.TableName}_{c.ColumnName}_seq'::regclass)";
                    }
                   
                }
                if (database == eDatabase.MsSqlServer)
                {
                    //veri tipi integer ise int ekle
                    if (c.DataType.ToLower().Contains("integer"))
                    {
                        columnDef += $"{c.ColumnName} INT";
                    }
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
                        columnDef += $" IDENTITY(1,1)";
                      
                    }

                }
             
                return columnDef;
            });
            // Primary Key tanımlamaları
            primaryKeys = columns
                .Where(c => c.IsPrimaryKey == "YES" ? true : false)
                .Select(c => c.ColumnName)
                .ToList();
            string primaryKeyConstraint = primaryKeys.Any()
                ? $", PRIMARY KEY ({string.Join(", ", primaryKeys)})"
                : string.Empty;


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
                eDatabase.MsSqlServer => $@"SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}') THEN 1 ELSE 0 END AS BIT) AS Exist;",

                eDatabase.Postgres => $"SELECT EXISTS ( SELECT 1 FROM information_schema.tables WHERE table_name = '{tableName}' ) AS Exist",

                eDatabase.Sqlite => $@"SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'",

                _ => throw new NotSupportedException($"The database type {database} is not supported.")
            };
            return query;
        }
        public static void TableSequenceCreate(List<TableSchema> sourceSchema,DbContext _destinationDbContext)
        {
          
                foreach (var schema in sourceSchema)
                {
                    if (schema.IsPrimaryKey == "YES")
                    {
                        string sequenceSql = $@"CREATE SEQUENCE {schema.TableName}_{schema.ColumnName}_seq";
                        var createSequence = _destinationDbContext.Database.ExecuteSqlRaw(sequenceSql);
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

            //burada primary key kontrolü sağlayıp fonksiyonda haber vermeliyiz
            string columnName = "";
            string data = "";
            topluData = "";// dışarıya gönderdiğimiz Data değeri
            topluColumn = "";// dışarıya gönderdiğimiz Column değeri
            columnName = key.ToString();
            data = value.ToString();
            data = "'" + data + "'";
            topluData = data;
            topluColumn = columnName;
            return "";
        }
    }
}