using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StreamShift.ApplicationContract.Enums;
using StreamShift.Persistence.Abstract;
using StreamShift.Persistence.ContextFactories.Abstract;
using StreamShift.Persistence.ContextFactories.Models;
using StreamShift.Persistence.Repository;
using System.Linq.Expressions;

namespace StreamShift.Persistence.Extension
{
    public static class RepositoryExt
    {
        public static IQueryable<TEntity> IncludeMultiple<TEntity>(this IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[] includes) where TEntity : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query;
        }

        public static IEnumerable<TEntity> ListAll<TEntity>(this IQueryable<TEntity> entities, Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            return predicate == null ? entities.AsNoTracking().ToList() : entities.Where(predicate).AsNoTracking().ToList();
        }

        public static void RegisterDbRepositories<TEntity>(this IServiceCollection services, DbConnectionOptions options) where TEntity : class
        {
            switch (options.SourceDbType)
            {
                case eDatabaseSource.Mongodb:
                    services.AddScoped<IRepository<TEntity>>(provider =>
                    {
                        var mongoContextFactory = provider.GetService<IMongoContextFactory>();
                        var database = mongoContextFactory.CreateMongoDatabase(options.SourceConnectionString, options.DatabaseName);
                        return new MongoRepository<TEntity>(database, typeof(TEntity).Name);
                    });
                    break;
                case eDatabaseSource.MsSqlServer:
                case eDatabaseSource.Postgres:
                case eDatabaseSource.Sqlite:
                    services.AddScoped<IRepository<TEntity>>(provider =>
                    {
                        var dbContextFactory = provider.GetService<IDbContextFactory>();
                        var dbContext = dbContextFactory.CreateDbContext(options.SourceConnectionString, options.SourceDbType);
                        return new MsSqlRepository<TEntity>(dbContext);
                    });
                    break;
                case eDatabaseSource.Redis:
                    services.AddScoped<IRepository<TEntity>>(provider =>
                    {
                        var redisContextFactory = provider.GetService<IRedisContextFactory>();
                        var connection = redisContextFactory.CreateRedisConnection(options.SourceConnectionString);
                        return new RedisRepository<TEntity>(connection, typeof(TEntity).Name);
                    });
                    break;
                //case eDatabase.Excel:
                //    services.AddScoped<IRepository<TEntity>>(provider =>
                //    {
                //        var excelContextFactory = provider.GetService<IExcelContextFactory>();
                //        var package = excelContextFactory.CreateExcelPackage("filePath");
                //        return new ExcelRepository<TEntity>(package);
                //    });
                //    break;
                case eDatabaseSource.Json:
                    services.AddScoped<IRepository<TEntity>>(provider =>
                    {
                        var jsonContextFactory = provider.GetService<IJsonContextFactory>();
                        var document = jsonContextFactory.CreateJsonDocument(typeof(TEntity).Name);
                        return new JsonRepository<TEntity>(document);
                    });
                    break;
                case eDatabaseSource.Xml:
                    services.AddScoped<IRepository<TEntity>>(provider =>
                    {
                        var xmlContextFactory = provider.GetService<IXmlContextFactory>();
                        var document = xmlContextFactory.CreateXDocument(typeof(TEntity).Name);
                        return new XmlRepository<TEntity>(document);
                    });
                    break;
                default:
                    throw new NotSupportedException($"The database type {options.SourceDbType} is not supported.");
            }
        }
    }
}