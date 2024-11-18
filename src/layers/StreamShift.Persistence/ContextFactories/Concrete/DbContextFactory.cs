using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using StreamShift.ApplicationContract.Enums;
using StreamShift.Persistence.ContextFactories.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StreamShift.Persistence.ContextFactories.Concrete
{
    public class DbContextFactory : IDbContextFactory
    {
        public DbContext CreateDbContext(string connectionString, eDatabase dbType)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DbContext>();

            switch (dbType)
            {
                case eDatabase.MsSqlServer:
                    optionsBuilder.UseSqlServer(connectionString);
                    break;
                case eDatabase.Postgres:
                    optionsBuilder.UseNpgsql(connectionString);
                    break;
                case eDatabase.Sqlite:
                    optionsBuilder.UseSqlite(connectionString);
                    break;
                default:
                    throw new NotSupportedException($"The database type {dbType} is not supported.");
            }

            return new DbContext(optionsBuilder.Options);
        }
    }

    public class MongoContextFactory : IMongoContextFactory
    {
        public IMongoDatabase CreateMongoDatabase(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            return client.GetDatabase(databaseName);
        }
    }

    public class RedisContextFactory : IRedisContextFactory
    {
        public ConnectionMultiplexer CreateRedisConnection(string connectionString)
        {
            return ConnectionMultiplexer.Connect(connectionString);
        }
    }

    //public class ExcelContextFactory : IExcelContextFactory
    //{
    //    public ExcelPackage CreateExcelPackage(string filePath)
    //    {
    //        var package = new ExcelPackage(new FileInfo(filePath));
    //        return package;
    //    }
    //}

    public class JsonContextFactory : IJsonContextFactory
    {
        public JsonDocument CreateJsonDocument(string jsonString)
        {
            return JsonDocument.Parse(jsonString);
        }
    }

    public class XmlContextFactory : IXmlContextFactory
    {
        public XDocument CreateXDocument(string xmlString)
        {
            return XDocument.Parse(xmlString);
        }
    }
}
