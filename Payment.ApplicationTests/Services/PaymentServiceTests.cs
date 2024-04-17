using Xunit;
using Moq;
using Payment.Infrastructure.Persistence.Repositories;
using Payment.Infrastructure;
using Payment.Application.Tests;
using Payment.Domain.Constants;
using FluentAssertions;
using Payment.Application.Dto;
using Payment.Application.Dto.Source;
using Payment.Event;

namespace Payment.Application.Services.Tests
{
    public class PaymentServiceTests
    {
        [Fact()]
        public async Task GetAsync_ShouldReturn_Payment()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Domain.Payment>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var paymentEventProducerMock = new Mock<IEventProducer<PaymentEvent>>();
            var payment = Fixture.NewPayment(PaymentType.OneTime);

            repositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(payment);
            unitOfWorkMock.Setup(x => x.CompleteAsync());

            // Act
            IPaymentService sut = new PaymentService(
                repositoryMock.Object,
                unitOfWorkMock.Object,
                paymentEventProducerMock.Object
                );

            var result = await sut.GetAsync(payment.Id);

            // Assert
            result!.Id.Should().Be(payment.Id);
            repositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<Guid>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Fact()]
        public async Task GetAsync_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Domain.Payment>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var paymentEventProducerMock = new Mock<IEventProducer<PaymentEvent>>();
            var payment = Fixture.NewPayment(PaymentType.OneTime);

            repositoryMock.Setup(x => x.FindByIdAsync(It.IsAny<Guid>()))
                .Throws<Exception>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());

            // Act
            IPaymentService sut = new PaymentService(
                repositoryMock.Object,
                unitOfWorkMock.Object,
                paymentEventProducerMock.Object
                );

            Func<Task> act = () => sut.GetAsync(payment.Id);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            repositoryMock.Verify(x => x.FindByIdAsync(It.IsAny<Guid>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
        }

        [Fact()]
        public async Task PayAsync_ShouldReturn_Payment()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Domain.Payment>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var paymentEventProducerMock = new Mock<IEventProducer<PaymentEvent>>();
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

            repositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Payment>()));
            unitOfWorkMock.Setup(x => x.CompleteAsync());

            // Act
            IPaymentService sut = new PaymentService(
                repositoryMock.Object,
                unitOfWorkMock.Object,
                paymentEventProducerMock.Object
                );

            var result = await sut.PayAsync(payment);

            // Assert
            result.Id.Should().NotBeEmpty();
            result.Status.Should().Be(PaymentStatus.EXECUTING);
            repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Payment>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Once);
        }

        [Fact()]
        public async Task PayAsync_ShouldThrowException_WhenRepositoryThrowsException()
        {
            // Arrange
            var repositoryMock = new Mock<IRepository<Domain.Payment>>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            var paymentEventProducerMock = new Mock<IEventProducer<PaymentEvent>>();
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

            repositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Payment>()))
                .Throws<Exception>();
            unitOfWorkMock.Setup(x => x.CompleteAsync());

            // Act
            IPaymentService sut = new PaymentService(
                repositoryMock.Object,
                unitOfWorkMock.Object,
                paymentEventProducerMock.Object
                );

            Func<Task> act = () => sut.PayAsync(payment);

            // Assert
            await act.Should().ThrowAsync<Exception>();
            repositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Payment>()), Times.Once);
            unitOfWorkMock.Verify(x => x.CompleteAsync(), Times.Never);
        }
    }
}