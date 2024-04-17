using Microsoft.Extensions.Logging;
using Payment.Infrastructure.Persistence.Repositories;
using Payment.Infrastructure;
using Payment.Event.PaymentEventProcessor;
using Microsoft.Extensions.Configuration;

namespace Payment.Event
{
    public class PaymentEventProcessorStrategyFactory(
        IRepository<Domain.Payment> paymentRepository,
        IUnitOfWork unitOfWork,
        HttpClient httpClient,
        IConfiguration configuration,
        ILoggerFactory loggerFactory) : IPaymentProcessorStrategyFactory
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IRepository<Domain.Payment> _paymentRepository = paymentRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;
        private readonly IConfiguration _configuration = configuration;

        public IPaymentEventProcessor CreateStrategy(EventType eventType)
        {
            return eventType switch
            {
                EventType.SendToBank => new SendPaymentEventProcessor(_paymentRepository, _unitOfWork, _httpClient, _configuration, _loggerFactory),
                EventType.ResponseFromBank => new ResponsePaymentEventProcessor(_paymentRepository, _unitOfWork, _loggerFactory),
                _ => throw new ArgumentException($"No stratey found for event type : {eventType}")
            };
        }
    }
}
