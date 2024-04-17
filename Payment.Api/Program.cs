using FluentValidation;
using FluentValidation.AspNetCore;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using IdempotentAPI.Core;
using IdempotentAPI.Extensions.DependencyInjection;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Payment.Api.Filter;
using Payment.Application.Services;
using Payment.Application.Validation;
using Payment.Bank.Stub;
using Payment.Event;
using Payment.Event.PaymentEventProcessor;
using Payment.Infrastructure;
using Payment.Infrastructure.Persistence.Repositories;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// TODO: Configure with actual Bank API
// Bank API Stub creation and setting base URL
var apiStub = BankApiStub.StartStub();
builder.Configuration.AddInMemoryCollection(
    new Dictionary<string, string?>
    {
        ["BankBaseUrl"] = apiStub.Address
    });

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.Converters.Add(new StringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<AddRequiredIdempotencyKeyAsHeader>();

});


// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PaymentValidator>();

// Dependencies
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRepository<Payment.Domain.Payment>, PaymentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IPaymentProcessorStrategyFactory, PaymentEventProcessorStrategyFactory>();
builder.Services.AddScoped<IEventProducer<PaymentEvent>, PaymentEventProducer>();

// HttpClient
builder.Services.AddHttpClient<IPaymentEventProcessor, SendPaymentEventProcessor>()
    .AddPolicyHandler(GetRetryPolicy());

// Mass Transit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PaymentEventConsumer>();

    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
        cfg.UseNewtonsoftRawJsonSerializer();
        cfg.UseNewtonsoftRawJsonDeserializer();
    });
});

var connectionString = @"Server=.;Database=Payment;TrustServerCertificate=True;Trusted_Connection=True";

builder.Services.AddDbContext<PaymentContext>(options =>
{
    options.UseSqlServer(connectionString);
    //options.UseInMemoryDatabase(databaseName: "Payments");
});

// API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
});

// Idempotency
IdempotencyOptions idempotencyOptions = new()
{
    CacheOnlySuccessResponses = true,
    DistributedLockTimeoutMilli = 2000,
};
builder.Services.AddIdempotentAPI(idempotencyOptions);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddIdempotentAPIUsingDistributedCache();

// Open Telemetry
// TODO: Configure Exporter
//builder.Services.AddOpenTelemetry()
//    .ConfigureResource(resourceBuilder => resourceBuilder.AddService("Payment Gateway"))
//    .WithTracing(builder => builder
//        .AddAspNetCoreInstrumentation()
//        .AddHttpClientInstrumentation()
//        .AddEntityFrameworkCoreInstrumentation()
//        .AddConsoleExporter())
//    .WithMetrics(builder => builder
//       .AddAspNetCoreInstrumentation()
//       .AddHttpClientInstrumentation()
//       .AddConsoleExporter());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
public partial class Program { }
