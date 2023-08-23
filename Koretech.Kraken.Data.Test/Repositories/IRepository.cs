using System.Linq.Expressions;

namespace Koretech.Kraken.Repositories
{
    public interface IRepository<T>
    {
        IQueryable<T> FindAll();
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
