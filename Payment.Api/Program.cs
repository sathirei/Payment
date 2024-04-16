using FluentValidation;
using FluentValidation.AspNetCore;
using IdempotentAPI.Cache.DistributedCache.Extensions.DependencyInjection;
using IdempotentAPI.Core;
using IdempotentAPI.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Converters;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Payment.Api.Filter;
using Payment.Application.Services;
using Payment.Application.Validation;
using Payment.Infrastructure;
using Payment.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

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

var connectionString = @"Server=.;Database=Payment;TrustServerCertificate=True;Trusted_Connection=True";

builder.Services.AddDbContext<PaymentContext>(options =>
{
    options.UseSqlServer(connectionString);
    //options.UseInMemoryDatabase(databaseName: "Payments");
});

// Versioning
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
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder => resourceBuilder.AddService("Payment Gateway"))
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddConsoleExporter())
    .WithMetrics(builder => builder
       .AddAspNetCoreInstrumentation()
       .AddHttpClientInstrumentation()
       .AddConsoleExporter());

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

public partial class Program { }
