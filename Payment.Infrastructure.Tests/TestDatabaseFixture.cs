using Microsoft.EntityFrameworkCore;

namespace Payment.Infrastructure.Tests
{
    public class TestDatabaseFixture
    {
        private const string ConnectionString = @"Server=.;Database=Payment;TrustServerCertificate=True;Trusted_Connection=True";

        private static readonly object _lock = new();
        private static bool _databaseInitialized;

        public TestDatabaseFixture()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public PaymentContext CreateContext() => new(
            new DbContextOptionsBuilder<PaymentContext>()
            //.UseInMemoryDatabase(databaseName: "TestPayment")
            .UseSqlServer(ConnectionString)
            .Options);
    }
}
