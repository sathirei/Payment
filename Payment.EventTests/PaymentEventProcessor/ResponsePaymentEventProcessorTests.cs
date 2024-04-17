using Xunit;
using Moq;
using Payment.Infrastructure.Persistence.Repositories;
using Payment.Infrastructure;
using Microsoft.Extensions.Logging;
using Payment.Event.Tests;
using Payment.Domain;
using FluentAssertions;

namespace Payment.Event.PaymentEventProcessor.Tests
{
    public class ResponsePaymentEventProcessorTests
    {
        [Fact()]
        public void ProcessAsync_Should_UpdateStatus_ToSuccess()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Domain.Payment>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            var loggerMock = new Mock<ILogger<ResponsePaymentEventProcessor>>();

            var id = Guid.NewGuid();
            var paymentEvent = new PaymentEvent
            {
                EventType = EventType.ResponseFromBank,
                Id = id,
                Payload = new BankResponse
                {
                    Id = id,
                    Status = "SUCCESS",
                    Message = "PAYMENT_COMPLETED"
                }
            };
            var newPayment = Fixture.NewPayment(Domain.Constants.PaymentType.OneTime);
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            repositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(newPayment);

            loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

            // Act
            var sut = new ResponsePaymentEventProcessor(repositoryMock.Object, unitOfWorkMock.Object, loggerFactoryMock.Object);
            var result = sut.ProcessAsync(paymentEvent);

            // Assert
            newPayment.Status.Should().Be(Domain.Constants.PaymentStatus.SUCCESS);
            newPayment.Response.Should().Be("PAYMENT_COMPLETED");
            repositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<Guid>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact()]
        public void ProcessAsync_Should_UpdateStatus_ToFailed()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Domain.Payment>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            var loggerMock = new Mock<ILogger<ResponsePaymentEventProcessor>>();

            var id = Guid.NewGuid();
            var paymentEvent = new PaymentEvent
            {
                EventType = EventType.ResponseFromBank,
                Id = id,
                Payload = new BankResponse
                {
                    Id = id,
                    Status = "FAILED",
                    Message = "SOMETHING_WENT_WRONG"
                }
            };
            var newPayment = Fixture.NewPayment(Domain.Constants.PaymentType.OneTime);
            unitOfWorkMock.Setup(x => x.CompleteAsync());
            repositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(newPayment);

            loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);

            // Act
            var sut = new ResponsePaymentEventProcessor(repositoryMock.Object, unitOfWorkMock.Object, loggerFactoryMock.Object);
            var result = sut.ProcessAsync(paymentEvent);

            // Assert
            newPayment.Status.Should().Be(Domain.Constants.PaymentStatus.FAILED);
            newPayment.Response.Should().Be("SOMETHING_WENT_WRONG");
            repositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<Guid>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
        }
    }
}