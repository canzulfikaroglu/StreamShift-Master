using MongoDB.Driver.Core.Configuration;
using Npgsql;
using StreamShift.Persistence.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StreamShift.Persistence.Repository
{
    public class PostgresRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public void Delete(TEntity model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<TEntity> FindByIdAsync(string id)
        {
            
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        public async Task InsertAsync(TEntity model)
        {
            //using var connection = new NpgsqlConnection(sourceConnection);
            //var tableName = typeof(TEntity).Name;

            //// TEntity'nin kolonlarını ve değerlerini çıkart
            //var columns = string.Join(", ", typeof(TEntity).GetProperties().Select(p => p.Name));
            //var values = string.Join(", ", typeof(TEntity).GetProperties().Select(p => $"@{p.Name}"));

            //var query = $"INSERT INTO {tableName} ({columns}) VALUES ({values})";

            //var command = new NpgsqlCommand(query, connection);

            //foreach (var prop in typeof(TEntity).GetProperties())
            //{
            //    command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(model) ?? DBNull.Value);
            //}

            //await connection.OpenAsync();
            //await command.ExecuteNonQueryAsync();
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> List(Expression<Func<TEntity, bool>> predicate = null)
        {
            throw new NotImplementedException();
        }

        public Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity model)
        {
            throw new NotImplementedException();
        }

    }
}
