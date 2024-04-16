using FluentAssertions;
using Payment.Domain.Constants;
using Payment.Infrastructure.Persistence.Repositories;

namespace Payment.Infrastructure.Tests.Repositories
{
    public class PaymentRepositoryTests : TestDatabaseFixture
    {
        [Fact()]
        public async Task Add_ShouldCreate_A_PaymentWithPlan()
        {
            // Arrange
            var newPayment = Fixture.NewPayment(PaymentType.Recurring, withPlan: true);
            var paymentContext = CreateContext();

            // Act
            var sut = new PaymentRepository(paymentContext);
            await sut.AddAsync(newPayment);
            await paymentContext.SaveChangesAsync();

            // Assert
            paymentContext.Payments!
                .FirstOrDefault(x => x.Id == newPayment.Id)
                .Should().NotBeNull();
        }

        [Fact()]
        public async Task FindByIdAsync_ShouldReturn_ThePayment()
        {
            // Arrange
            var newPayment = Fixture.NewPayment(PaymentType.Recurring, withPlan: true);
            var paymentContext = CreateContext();

            // Act
            var sut = new PaymentRepository(paymentContext);
            await sut.AddAsync(newPayment);
            await paymentContext.SaveChangesAsync();
            var paymentInDatabase = await sut.FindByIdAsync(newPayment.Id);

            // Assert
            paymentInDatabase.Should().NotBeNull();
        }

        [Fact()]
        public async Task FindAsync_ShouldReturn_ThePayment()
        {
            // Arrange
            var newPayment = Fixture.NewPayment(PaymentType.Recurring, withPlan: true);
            var paymentContext = CreateContext();

            // Act
            var sut = new PaymentRepository(paymentContext);
            await sut.AddAsync(newPayment);
            await paymentContext.SaveChangesAsync();
            var paymentInDatabase = await sut.FindAsync(x => x.Id == newPayment.Id);

            // Assert
            paymentInDatabase.Should().NotBeNull();
        }

        [Fact()]
        public async Task Remove_ShouldDelete_ThePayment()
        {
            // Arrange
            var newPayment = Fixture.NewPayment(PaymentType.Recurring, withPlan: true);
            var paymentContext = CreateContext();

            // Act
            var sut = new PaymentRepository(paymentContext);
            await sut.AddAsync(newPayment);
            await paymentContext.SaveChangesAsync();
            var paymentInDatabase = await sut.FindByIdAsync(newPayment.Id);
            sut.Remove(paymentInDatabase!);
            await paymentContext.SaveChangesAsync();
            var deletedPayment = await sut.FindByIdAsync(newPayment.Id);

            // Assert
            deletedPayment.Should().BeNull();
        }

        [Fact()]
        public async Task Update_ShouldUpdate_ThePayment()
        {
            // Arrange
            var newPayment = Fixture.NewPayment(PaymentType.Recurring, withPlan: true);
            var paymentContext = CreateContext();

            // Act
            var sut = new PaymentRepository(paymentContext);
            await sut.AddAsync(newPayment);
            await paymentContext.SaveChangesAsync();
            var paymentInDatabase = await sut.FindByIdAsync(newPayment.Id);
            paymentInDatabase!.UpdateStatus(PaymentStatus.SUCCESS);
            paymentContext.Update(paymentInDatabase!);
            await paymentContext.SaveChangesAsync();
            var updatedPayment = await sut.FindByIdAsync(newPayment.Id);

            // Assert
            updatedPayment!.Status.Should().Be(PaymentStatus.SUCCESS);
        }
    }
}
