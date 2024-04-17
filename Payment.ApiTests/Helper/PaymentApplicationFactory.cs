using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Payment.Bank.Stub;
using Payment.Infrastructure;

namespace Payment.ApiTests.Helper
{
    public class PaymentApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // TODO: scope and dispose apiInstance on the running test
            var apiInstance = BankApiStub.StartStub();
            base.ConfigureWebHost(builder);

            builder.ConfigureTestServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<PaymentContext>));
                services.Remove(dbContextDescriptor);

                services.AddDbContext<PaymentContext>((container, options) =>
                {
                    options.UseInMemoryDatabase(databaseName: "Payment_Tests");
                });
            });

            builder.ConfigureAppConfiguration((ctx, b) =>
            {
                b.Add(new MemoryConfigurationSource
                {
                    InitialData = new Dictionary<string, string?>
                    {
                        ["BankBaseUrl"] = apiInstance.Address
                    }
                });
            });

            builder.UseEnvironment("Development");
        }
    }
}
