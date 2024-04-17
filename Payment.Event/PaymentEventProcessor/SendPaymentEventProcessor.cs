using Microsoft.Extensions.Logging;
using Payment.Infrastructure.Persistence.Repositories;
using Payment.Infrastructure;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Payment.Event.PaymentEventProcessor
{
    public class SendPaymentEventProcessor(
        IRepository<Domain.Payment> paymentRepository,
        IUnitOfWork unitOfWork,
        HttpClient httpClient,
        IConfiguration configuration,
        ILoggerFactory loggerFactory) : IPaymentEventProcessor
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IRepository<Domain.Payment> _paymentRepository = paymentRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<SendPaymentEventProcessor> _logger = loggerFactory.CreateLogger<SendPaymentEventProcessor>();

        public async Task ProcessAsync(PaymentEvent paymentEvent)
        {
            try
            {
                var bankBaseUrl = _configuration.GetSection("BankBaseUrl").Value;
                _logger.LogInformation("Processing {EventType} for {CorrelationId}", paymentEvent.EventType, paymentEvent.Id);
                var response = await _httpClient.PostAsJsonAsync(bankBaseUrl + "/Payment", paymentEvent.Payload);
                var initialBankResponse = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                var existingPayment = await _paymentRepository.FindByIdAsync(paymentEvent.Id);
                existingPayment!.UpdateResponse(initialBankResponse);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Processing {EventType} completed for {CorrelationId}", paymentEvent.EventType, paymentEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Processing {EventType} failed {CorrelationId}", paymentEvent.EventType, paymentEvent.Id);
                // TODO: Retry and Dead Letter
                _logger.LogError(ex, "Error while sending payment to bank.");
                throw;
            }
        }
    }
}
