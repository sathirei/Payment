using Xunit;
using Payment.Event;
using Payment.Infrastructure.Persistence.Repositories;
using Payment.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Payment.EventTests.Helper;
using FluentAssertions;
using Payment.Event.PaymentEventProcessor;
using Microsoft.Extensions.Configuration;

namespace Payment.EventTests
{
    public class PaymentEventProcessorStrategyFactoryTests
    {
        [Fact()]
        public void CreateStrategy_ShouldReturn_SendPaymentEventProcessor()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Domain.Payment>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            var mockHttpMessageHandler = MockHttpMessageHandler<string>.SetUpResource("Test");
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var configurationMock = new Mock<IConfiguration>();

            // Act
            var sut = new PaymentEventProcessorStrategyFactory(
                repositoryMock.Object,
                unitOfWorkMock.Object,
                httpClient,
                configurationMock.Object,
                loggerFactoryMock.Object);

            var result = sut.CreateStrategy(EventType.SendToBank);

            // Assert
            result.Should().BeOfType<SendPaymentEventProcessor>();
        }

        [Fact()]
        public void CreateStrategy_ShouldReturn_ResponsePaymentEventProcessor()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Domain.Payment>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            var mockHttpMessageHandler = MockHttpMessageHandler<string>.SetUpResource("Test");
            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var configurationMock = new Mock<IConfiguration>();

            // Act
            var sut = new PaymentEventProcessorStrategyFactory(
                repositoryMock.Object,
                unitOfWorkMock.Object,
                httpClient,
                configurationMock.Object,
                loggerFactoryMock.Object);

            var result = sut.CreateStrategy(EventType.ResponseFromBank);

            // Assert
            result.Should().BeOfType<ResponsePaymentEventProcessor>();
        }
    }
}