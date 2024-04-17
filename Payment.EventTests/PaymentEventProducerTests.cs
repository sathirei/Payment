using Xunit;
using Moq;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Payment.Event.Tests
{
    public class PaymentEventProducerTests
    {
        [Fact()]
        public async Task ProduceAsync_ShouldPublishToBus()
        {
            // Arrange
            var busMock = new Mock<IBus>();
            var loggerMock = new Mock<ILogger<PaymentEventProducer>>();
            var paymentEvent = new PaymentEvent
            {
                EventType = EventType.SendToBank,
                Id = Guid.NewGuid(),
                Payload = ""
            };

            busMock.Setup(x => x.Publish(It.IsAny<PaymentEvent>(), It.IsAny<CancellationToken>()));

            // Act
            var sut = new PaymentEventProducer(busMock.Object, loggerMock.Object);
            await sut.ProduceAsync(paymentEvent);

            // Assert
            busMock.Verify(x => x.Publish(It.IsAny<PaymentEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}