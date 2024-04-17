using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Payment.Domain;
using Payment.Domain.Constants;
using Payment.Infrastructure;
using Payment.Infrastructure.Persistence.Repositories;

namespace Payment.Event.PaymentEventProcessor
{
    public class ResponsePaymentEventProcessor(
        IRepository<Domain.Payment> paymentRepository,
        IUnitOfWork unitOfWork,
        ILoggerFactory loggerFactory) : IPaymentEventProcessor
    {
        private readonly IRepository<Domain.Payment> _paymentRepository = paymentRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILogger<ResponsePaymentEventProcessor> _logger = loggerFactory.CreateLogger<ResponsePaymentEventProcessor>();

        public async Task ProcessAsync(PaymentEvent paymentEvent)
        {
            try
            {
                _logger.LogInformation("Processing {EventType} for {CorrelationId}", paymentEvent.EventType, paymentEvent.Id);
                var existingPayment = await _paymentRepository.FindByIdAsync(paymentEvent.Id);
                var response = JsonConvert.DeserializeObject<BankResponse>(paymentEvent.Payload.ToString());
                if (response?.Status == "SUCCESS")
                {
                    existingPayment!.UpdateStatus(PaymentStatus.SUCCESS);
                }
                else
                {
                    existingPayment!.UpdateStatus(PaymentStatus.FAILED);
                }
                existingPayment!.UpdateResponse(response?.Message);

                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Processing {EventType} completed for {CorrelationId}", paymentEvent.EventType, paymentEvent.Id);
            }
            catch (Exception ex)
            {
                // TODO: Retry and Dead Letter
                _logger.LogInformation("Processing {EventType} failed for {CorrelationId}", paymentEvent.EventType, paymentEvent.Id);
                _logger.LogError(ex, "Error while processing bank's response.");
                throw;
            }
        }
    }
}
