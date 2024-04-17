using MassTransit;
using Microsoft.Extensions.Logging;

namespace Payment.Event
{

    public class PaymentEventConsumer(
        IPaymentProcessorStrategyFactory strategyFactory,
        ILogger<PaymentEventConsumer> logger) : IConsumer<PaymentEvent>
    {
        readonly ILogger<PaymentEventConsumer> _logger = logger;
        readonly IPaymentProcessorStrategyFactory _strategyFactory = strategyFactory;

        public async Task Consume(ConsumeContext<PaymentEvent> context)
        {
            var paymentEvent = context.Message;
            try
            {
                _logger.LogInformation("Received Payment Event: {EventType}, PaymentId: {CorrelationId}", paymentEvent.EventType, paymentEvent.Id);
                var strategy = _strategyFactory.CreateStrategy(paymentEvent.EventType);
                await strategy.ProcessAsync(paymentEvent);
            }
            catch (Exception ex)
            {
                // TODO: Handle retry
                _logger.LogInformation("Failed to consume Payment Event: {EventType}, PaymentId: {CorrelationId}", paymentEvent.EventType, paymentEvent.Id);
                _logger.LogError(ex, "Error while consuming payment event");
            }
        }
    }
}
