using Xunit;
using Payment.Application.Services;
using Moq;
using Microsoft.Extensions.Logging;
using Payment.Application.Dto.Source;
using Payment.Application.Dto;
using Payment.Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using Payment.Domain;

namespace Payment.Api.Controllers.Tests
{
    public class PaymentControllerTests
    {
        [Fact()]
        public async Task PostAsync_ShouldReturn_Accepted()
        {
            // Arrange
            var paymentServicMock = new Mock<IPaymentService>();
            var loggerMock = new Mock<ILogger<PaymentController>>();
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = 02,
                    ExpiryYear = 2025
                },
                Amount = 10000,
                MerchantId = "123",
                Currency = "GBP",
                Reference = "123_456",
                Type = PaymentType.OneTime
            };
            var paymentResult = new PaymentResultDto
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.EXECUTING
            };

            paymentServicMock.Setup(x => x.PayAsync(It.IsAny<PaymentDto>()))
                .ReturnsAsync(paymentResult);

            // Act
            var sut = new PaymentController(paymentServicMock.Object, loggerMock.Object);

            var result = await sut.PostAsync(payment) as AcceptedResult;

            // Assert
            result!.StatusCode.Should().Be(202);
            paymentServicMock.Verify(x => x.PayAsync(It.IsAny<PaymentDto>()), Times.Once);
        }

        [Fact()]
        public async Task PostAsync_ShouldReturn_PaymentResult()
        {
            // Arrange
            var paymentServicMock = new Mock<IPaymentService>();
            var loggerMock = new Mock<ILogger<PaymentController>>();
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = 02,
                    ExpiryYear = 2025
                },
                Amount = 10000,
                MerchantId = "123",
                Currency = "GBP",
                Reference = "123_456",
                Type = PaymentType.OneTime
            };
            var paymentResult = new PaymentResultDto
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.EXECUTING
            };

            paymentServicMock.Setup(x => x.PayAsync(It.IsAny<PaymentDto>()))
                .ReturnsAsync(paymentResult);

            // Act
            var sut = new PaymentController(paymentServicMock.Object, loggerMock.Object);

            var result = await sut.PostAsync(payment) as AcceptedResult;

            // Assert
            result!.StatusCode.Should().Be(202);
            result.Value.Should().BeAssignableTo<PaymentResultDto>()
                .Which.Should().BeEquivalentTo(paymentResult);
            paymentServicMock.Verify(x => x.PayAsync(It.IsAny<PaymentDto>()), Times.Once);
        }

        [Fact()]
        public async Task PostAsync_ShouldThrowAnException_WhenServiceThrowsAnException()
        {
            // Arrange
            var paymentServicMock = new Mock<IPaymentService>();
            var loggerMock = new Mock<ILogger<PaymentController>>();
            var payment = new PaymentDto
            {
                Source = new PaymentSourceDto
                {
                    Type = PaymentSourceType.CreditCard,
                    ExpiryMonth = 02,
                    ExpiryYear = 2025
                },
                Amount = 10000,
                MerchantId = "123",
                Currency = "GBP",
                Reference = "123_456",
                Type = PaymentType.OneTime
            };
            var paymentResult = new PaymentResultDto
            {
                Id = Guid.NewGuid(),
                Status = PaymentStatus.EXECUTING
            };

            paymentServicMock.Setup(x => x.PayAsync(It.IsAny<PaymentDto>()))
                .Throws(new Exception());

            // Act
            var sut = new PaymentController(paymentServicMock.Object, loggerMock.Object);

            Func<Task> act = async () => await sut.PostAsync(payment);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            paymentServicMock.Verify(x => x.PayAsync(It.IsAny<PaymentDto>()), Times.Once);
        }

        [Fact()]
        public async Task GetAsync_ShouldReturn_Ok()
        {
            // Arrange
            var paymentServicMock = new Mock<IPaymentService>();
            var loggerMock = new Mock<ILogger<PaymentController>>();
            var paymentView = new PaymentViewDto(
                Guid.NewGuid(),
                PaymentStatus.EXECUTING,
                "1111222233334444",
                PaymentType.OneTime,
                1000,
                "GBP",
                "Amazon_123",
                "23455",
                "Response",
                SystemTime.OffsetNow(),
                SystemTime.OffsetNow()
                );

            paymentServicMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(paymentView);

            // Act
            var sut = new PaymentController(paymentServicMock.Object, loggerMock.Object);

            var result = await sut.GetAsync(paymentView.Id) as OkObjectResult;

            // Assert
            result!.StatusCode.Should().Be(200);
            paymentServicMock.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact()]
        public async Task GetAsync_ShouldReturn_NotFound()
        {
            // Arrange
            var paymentServicMock = new Mock<IPaymentService>();
            var loggerMock = new Mock<ILogger<PaymentController>>();

            paymentServicMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(default(PaymentViewDto));

            // Act
            var sut = new PaymentController(paymentServicMock.Object, loggerMock.Object);

            var result = await sut.GetAsync(Guid.NewGuid()) as NotFoundResult;

            // Assert
            result!.StatusCode.Should().Be(404);
            paymentServicMock.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact()]
        public async Task GetAsync_ShouldReturn_PaymentViewResult()
        {
            // Arrange
            var paymentServicMock = new Mock<IPaymentService>();
            var loggerMock = new Mock<ILogger<PaymentController>>();
            var paymentView = new PaymentViewDto(
                Guid.NewGuid(),
                PaymentStatus.EXECUTING,
                "1111222233334444",
                PaymentType.OneTime,
                1000,
                "GBP",
                "Amazon_123",
                "23455",
                "Response",
                SystemTime.OffsetNow(),
                SystemTime.OffsetNow()
                );

            paymentServicMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(paymentView);

            // Act
            var sut = new PaymentController(paymentServicMock.Object, loggerMock.Object);

            var result = await sut.GetAsync(paymentView.Id) as OkObjectResult;

            // Assert
            result!.StatusCode.Should().Be(200);
            paymentServicMock.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
            result.Value.Should().BeAssignableTo<PaymentViewDto>()
                .Which.Should().BeEquivalentTo(paymentView);
        }

        [Fact()]
        public async Task GetAsync_ShouldThrowAnException_WhenServiceThrowsAnException()
        {
            // Arrange
            var paymentServicMock = new Mock<IPaymentService>();
            var loggerMock = new Mock<ILogger<PaymentController>>();

            paymentServicMock.Setup(x => x.GetAsync(It.IsAny<Guid>()))
                .Throws(new Exception());

            // Act
            var sut = new PaymentController(paymentServicMock.Object, loggerMock.Object);

            Func<Task> act = async () => await sut.GetAsync(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<Exception>();
            paymentServicMock.Verify(x => x.GetAsync(It.IsAny<Guid>()), Times.Once);
        }
    }
}