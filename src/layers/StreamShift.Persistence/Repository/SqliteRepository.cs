using StreamShift.Persistence.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StreamShift.Persistence.Repository
{
    public class SqliteRepository<TEntity> : IRepository<TEntity> where TEntity : class
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
