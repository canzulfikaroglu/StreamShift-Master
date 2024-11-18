using Microsoft.EntityFrameworkCore;
using StreamShift.Persistence.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StreamShift.Persistence.Repository
{
    public class MsSqlRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private DbContext _dbContext;
        public DbSet<TEntity> _dbSet { get; set; }

        public MsSqlRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

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

        public Task InsertAsync(TEntity model)
        {
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
