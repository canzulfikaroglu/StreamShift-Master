using Microsoft.Extensions.DependencyInjection;
using StreamShift.Persistence.Abstract;
namespace StreamShift.Persistence.Repository
{
    public static class RegisterRepositoriesExtension
    {
        public static void RegisterRepositoryServices<TEntity>(this IServiceCollection services) where TEntity : class
        {
            services.AddScoped(typeof(IRepository<TEntity>), typeof(Repository<TEntity>));
            services.AddScoped(typeof(IRepository<TEntity>), typeof(ExcelRepository<TEntity>));
            services.AddScoped(typeof(IRepository<TEntity>), typeof(RedisRepository<TEntity>));
            services.AddScoped(typeof(IRepository<TEntity>), typeof(MsSqlRepository<TEntity>));
            services.AddScoped(typeof(IRepository<TEntity>), typeof(PostgresRepository<TEntity>));
            services.AddScoped(typeof(IRepository<TEntity>), typeof(SqliteRepository<TEntity>));
            services.AddScoped(typeof(IRepository<TEntity>), typeof(JsonRepository<TEntity>));
            services.AddScoped(typeof(IRepository<TEntity>), typeof(XmlRepository<TEntity>));
        }
    }
}