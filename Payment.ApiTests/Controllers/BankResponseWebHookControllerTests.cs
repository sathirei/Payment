using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Payment.Application.Dto;
using Payment.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Payment.Event;

namespace Payment.Api.Controllers.Tests
{
    public class BankResponseWebHookControllerTests
    {
        [Fact()]
        public async Task PostAsync_ShouldReturn_Accepted()
        {
            // Arrange
            var paymentEventProducerMock = new Mock<IEventProducer<PaymentEvent>>();
            var loggerMock = new Mock<ILogger<BankResponseWebHookController>>();
            var bankResponse = new Domain.BankResponse
            {
                Id = Guid.NewGuid(),
                Status = "SUCCESS",
                Message = "PAYMENT_COMPLETED"
            };
            var paymentResult = new PaymentResultDto
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.EXECUTING
            };

            paymentEventProducerMock.Setup(x => x.ProduceAsync(It.IsAny<PaymentEvent>()));

            // Act
            var sut = new BankResponseWebHookController(paymentEventProducerMock.Object, loggerMock.Object);

            var result = await sut.PostAsync(bankResponse) as AcceptedResult;

            // Assert
            result!.StatusCode.Should().Be(202);
            paymentEventProducerMock.Verify(x => x.ProduceAsync(It.IsAny<PaymentEvent>()), Times.Once);
        }

        [Fact()]
        public async Task PostAsync_ShouldThrowAnException_WhenServiceThrowsAnException()
        {
            // Arrange
            var paymentEventProducerMock = new Mock<IEventProducer<PaymentEvent>>();
            var loggerMock = new Mock<ILogger<BankResponseWebHookController>>();
            var bankResponse = new Domain.BankResponse
            {
                Id = Guid.NewGuid(),
                Status = "SUCCESS",
                Message = "PAYMENT_COMPLETED"
            };
            var paymentResult = new PaymentResultDto
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.EXECUTING
            };

            paymentEventProducerMock.Setup(x => x.ProduceAsync(It.IsAny<PaymentEvent>()))
                .Throws<Exception>();

            // Act
            var sut = new BankResponseWebHookController(paymentEventProducerMock.Object, loggerMock.Object);

            Func<Task> act = async () => await sut.PostAsync(bankResponse);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            paymentEventProducerMock.Verify(x => x.ProduceAsync(It.IsAny<PaymentEvent>()), Times.Once);
        }
    }
}