using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using StreamShift.Domain.DatabaseEnums;
using System.Text.Json;
using System.Xml.Linq;

namespace StreamShift.Infrastructure.ContextFactories.Abstract
{
    public interface IDbContextFactory
    {
        DbContext CreateDbContext(string connectionString, eDatabase dbType);
    }

    public interface IMongoContextFactory
    {
        IMongoDatabase CreateMongoDatabase(string connectionString, string databaseName);
    }

    public interface IRedisContextFactory
    {
        ConnectionMultiplexer CreateRedisConnection(string connectionString);
    }

    //public interface IExcelContextFactory
    //{
    //    ExcelPackage CreateExcelPackage(string filePath);
    //}

    public interface IJsonContextFactory
    {
        JsonDocument CreateJsonDocument(string jsonString);
    }

    public interface IXmlContextFactory
    {
        XDocument CreateXDocument(string xmlString);
    }
}
