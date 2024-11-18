using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using StreamShift.ApplicationContract.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StreamShift.Persistence.ContextFactories.Abstract
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
