using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using MassTransit;

namespace Payment.Event.Tests
{
    public class PaymentEventConsumerTests
    {
        [Fact()]
        public async Task Consume_Should_CreateAppropriateStrategy_And_Process()
        {
            // Arrange
            var strategyFactoryMock = new Mock<IPaymentProcessorStrategyFactory>();
            var strategyMock = new Mock<IPaymentEventProcessor>();
            var loggerMock = new Mock<ILogger<PaymentEventConsumer>>();
            var contextMock = new Mock<ConsumeContext<PaymentEvent>>();
            contextMock.SetupGet(x => x.Message).Returns(new PaymentEvent
            {
                Id = Guid.NewGuid(),
                EventType = EventType.SendToBank,
                Payload = ""
            });

            strategyFactoryMock.Setup(x => x.CreateStrategy(It.IsAny<EventType>())).Returns(strategyMock.Object);

            // Act
            var sut = new PaymentEventConsumer(strategyFactoryMock.Object, loggerMock.Object);
            await sut.Consume(contextMock.Object);

            // Assert
            contextMock.VerifyGet(x => x.Message, Times.Once);
            strategyFactoryMock.Verify(x => x.CreateStrategy(It.IsAny<EventType>()), Times.Once);
        }
    }
}