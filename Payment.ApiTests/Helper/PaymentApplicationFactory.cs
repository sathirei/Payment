using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Payment.Application.Services;
using Payment.Infrastructure;
using Payment.Infrastructure.Persistence.Repositories;

namespace Payment.ApiTests.Helper
{
    public class PaymentApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
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
                //var paymentservice = services.SingleOrDefault(
                //   d => d.ServiceType == typeof(PaymentService));
                //services.Remove(paymentservice);
                //services.AddScoped<IUnitOfWork, UnitOfWork>();
                //services.AddScoped<IRepository<Domain.Payment>, PaymentRepository>();

                //services.AddScoped<IPaymentService, PaymentService>(builder =>
                //{
                //    var repository = builder.GetRequiredService<IRepository<Domain.Payment>>();
                //    var unitOfWork = builder.GetRequiredService<IUnitOfWork>();
                //    // var mock = MockHttpMessageHandler<int>.SetUpBasicGetResource(new List<int> { 1, 2, 3 });
                //    return new PaymentService(repository, unitOfWork);
                //});
            });

            builder.UseEnvironment("Development");
        }
    }
}
