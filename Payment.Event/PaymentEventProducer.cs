using MassTransit;
using Microsoft.Extensions.Logging;

namespace Payment.Event
{
    public class PaymentEventProducer(IBus bus, ILogger<PaymentEventProducer> logger) : IEventProducer<PaymentEvent>
    {
        private readonly IBus _bus = bus;
        private readonly ILogger<PaymentEventProducer> _logger = logger;

        public async Task ProduceAsync(PaymentEvent paymentEvent)
        {
            _logger.LogInformation("Producing event {EventType} for {CorrelationId}", paymentEvent.EventType, paymentEvent.Id);
            await _bus.Publish(paymentEvent);
        }
    }
}
