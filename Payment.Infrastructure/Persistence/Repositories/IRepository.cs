using Payment.Domain;
using System.Linq.Expressions;

namespace Payment.Infrastructure.Persistence.Repositories
{
    public interface IRepository<T>
        where T : class, IEntityMarker
    {
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> FindByIdAsync(Guid id);
        Task AddAsync(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}
