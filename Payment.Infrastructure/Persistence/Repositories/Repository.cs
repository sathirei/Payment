using Microsoft.EntityFrameworkCore;
using Payment.Domain;
using System.Linq.Expressions;

namespace Payment.Infrastructure.Persistence.Repositories
{
    public abstract class Repository<T> : IRepository<T>
         where T : class, IEntityMarker
    {
        protected readonly PaymentContext _context;

        public Repository(PaymentContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public virtual async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            var result = await _context.Set<T>()
                .FirstOrDefaultAsync(predicate)
                .ConfigureAwait(false);

            return result;
        }

        public virtual async Task<T?> FindByIdAsync(Guid id)
        {
            var result = await _context.Set<T>()
                .FindAsync(id)
                .ConfigureAwait(false);

            return result;
        }
    }
}
