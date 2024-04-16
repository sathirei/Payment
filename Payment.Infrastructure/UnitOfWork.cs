using Microsoft.Extensions.Logging;

namespace Payment.Infrastructure
{
    public class UnitOfWork(
        PaymentContext context,
        ILogger<UnitOfWork> logger
        ) : IUnitOfWork, IDisposable
    {
        private readonly PaymentContext _context = context;
        private readonly ILogger<UnitOfWork> _logger = logger;

        public async Task CompleteAsync()
        {
            _logger.LogInformation("Completing unit of work.");
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
        }

    }
}
