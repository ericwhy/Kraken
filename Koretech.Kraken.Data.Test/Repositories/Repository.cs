using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Koretech.Kraken.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext _dbContext;

        public Repository(DbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public IQueryable<TEntity> FindAll()
        {
            return this._dbContext.Set<TEntity>().AsNoTracking();
        }

        public IQueryable<TEntity> FindByCondition(Expression<Func<TEntity, bool>> expression)
        {
            return this._dbContext.Set<TEntity>()
                .Where(expression).AsNoTracking();
        }

        public void Insert(TEntity entity)
        {
            this._dbContext.Set<TEntity>().Add(entity);
        }

        public void Update(TEntity entity)
        {
            this._dbContext.Set<TEntity>().Update(entity);
        }

        public void Delete(TEntity entity)
        {
            this._dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
