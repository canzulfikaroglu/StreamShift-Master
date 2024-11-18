using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StreamShift.Persistence.Abstract
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> FindByIdAsync(string id);
        IEnumerable<TEntity> List(Expression<Func<TEntity, bool>> predicate = null);
        Task InsertAsync(TEntity model);
        void Update(TEntity model);
        void Delete(TEntity model);
        Task DeleteAsync(string id);
        Task SaveChangesAsync();
        IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includes);
    }
}
