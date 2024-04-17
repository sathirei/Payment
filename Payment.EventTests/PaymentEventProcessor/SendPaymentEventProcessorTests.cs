using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using Payment.Infrastructure.Persistence.Repositories;
using Payment.Infrastructure;
using Payment.EventTests.Helper;
using Microsoft.Extensions.Configuration;
using Payment.Event.Tests;
using FluentAssertions;

namespace Payment.Event.PaymentEventProcessor.Tests
{
    public class SendPaymentEventProcessorTests
    {
        [Fact()]
        public async Task ProcessAsync_Should_SendPaymentToBank()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Domain.Payment>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            var loggerMock = new Mock<ILogger<SendPaymentEventProcessor>>();
            var mockHttpMessageHandler = MockHttpMessageHandler<string>.SetUpResource("Bank Response");
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var inMemorySettings = new Dictionary<string, string>
            {
                { "BankBaseUrl", "http://bank:9090" }
            };
            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var newPayment = Fixture.NewPayment(Domain.Constants.PaymentType.OneTime);
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            repositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(newPayment);

            loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

            var paymentEvent = new PaymentEvent
            {
                EventType = EventType.SendToBank,
                Id = Guid.NewGuid(),
                Payload = "Test"
            };


            // Act
            var sut = new SendPaymentEventProcessor(
                repositoryMock.Object,
                unitOfWorkMock.Object,
                httpClient,
                configuration,
                loggerFactoryMock.Object
                );

            await sut.ProcessAsync(paymentEvent);

            // Assert
            newPayment.Response.Should().Be("\"Bank Response\"");
            unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
            repositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}