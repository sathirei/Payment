using FluentAssertions;

namespace Payment.Domain.Tests
{
    public class PaymentPlanTests
    {
        [Fact()]
        public void PaymentPlan_Should_SetFields()
        {
            // Arrange
            var daysBetweenPayment = 30;
            var totalNumberOfPayments = 6;
            var currentPaymentNumber = 1;
            var expiry = new DateTime(2024, 10, 31);

            // Act
            var sut = new PaymentPlan(
                daysBetweenPayment,
                totalNumberOfPayments,
                currentPaymentNumber,
                expiry
                );

            // Assert
            sut.DaysBetweenPayments.Should().Be(daysBetweenPayment);
            sut.TotalNumberOfPayments.Should().Be(totalNumberOfPayments);
            sut.CurrentPaymentNumber.Should().Be(currentPaymentNumber);
            sut.Expiry.Should().Be(expiry);
        }
    }
}
