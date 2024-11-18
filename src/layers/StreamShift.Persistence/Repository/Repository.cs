using Microsoft.EntityFrameworkCore;
using StreamShift.Persistence.Abstract;
using StreamShift.Persistence.Context;
using StreamShift.Persistence.Extension;
using System.Linq.Expressions;

namespace StreamShift.Persistence.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public DbSet<TEntity> _dbSet { get; set; }
        public readonly AppDb _dbContext;

        public Repository(AppDb dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        public void Delete(TEntity model)
        {
            _dbSet.Remove(model);
        }

        /// <summary>
        /// Bu metod ile alınan entity'ler delete edilemez, tracking edilmeden alınan entityler delete edilebilir
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> FindByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task InsertAsync(TEntity model)
        {
            await _dbSet.AddAsync(model);
        }

        public IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includes)
        {
            return RepositoryExt.IncludeMultiple(_dbSet.AsQueryable(), includes);
        }

        public IEnumerable<TEntity> List(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate == null ? _dbSet.AsNoTracking().ToList() : _dbSet.Where(predicate).AsNoTracking().ToList();
        }

        public void Update(TEntity model)
        {
            _dbSet.Update(model);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            await _dbSet.Where(w => EF.Property<string>(w, "Id") == id).ExecuteDeleteAsync();
        }
    }
}