using Payment.Domain.Tests;
using Payment.Domain.Constants;
using FluentAssertions;

namespace Payment.Domain.BusinessRules.Tests
{
    public class OnlyRecurringPaymentCanHavePaymentPlanTests
    {
        [Fact()]
        public void OnlyRecurringPaymentCanHavePaymentPlan_ShouldSetCodeAndMessage()
        {
            // Arrange
            var payment = Fixture.NewPayment(PaymentType.OneTime, withPlan: false);

            // Act
            var sut = new OnlyRecurringPaymentCanHavePaymentPlan(payment);

            // Assert
            sut.Code.Should().Be("payment-plan-only-valid-for-recurring-payment-type");
            sut.BrokenRuleMessage.Should()
                .Be("Payment plan can only be associated with 'Recurring' payment type.");
        }

        [Fact()]
        public void IsValid_ShouldReturnTrue_WhenPaymentTypeIsOneTime()
        {
            // Arrange
            var payment = Fixture.NewPayment(PaymentType.OneTime, withPlan: false);

            // Act
            var sut = new OnlyRecurringPaymentCanHavePaymentPlan(payment);

            // Assert
            sut.IsValid().Should().BeTrue();
        }

        [Fact()]
        public void IsValid_ShouldReturnFalse_WhenPaymentTypeIsOneTime_AndPaymentPlanIsProvided()
        {
            // Arrange
            var payment = Fixture.NewPayment(PaymentType.OneTime, withPlan: true);

            // Act
            var sut = new OnlyRecurringPaymentCanHavePaymentPlan(payment);

            // Assert
            sut.IsValid().Should().BeFalse();
        }

        [Fact()]
        public void IsValid_ShouldReturnTrue_WhenPaymentTypeIsRecurring_AndPaymentPlanIsProvided()
        {
            // Arrange
            var payment = Fixture.NewPayment(PaymentType.Recurring, withPlan: true);

            // Act
            var sut = new OnlyRecurringPaymentCanHavePaymentPlan(payment);

            // Assert
            sut.IsValid().Should().BeTrue();
        }

        [Fact()]
        public void IsValid_ShouldReturnFalse_WhenPaymentTypeIsRecurring_AndPaymentPlanIsNotProvided()
        {
            // Arrange
            var payment = Fixture.NewPayment(PaymentType.Recurring, withPlan: false);

            // Act
            var sut = new OnlyRecurringPaymentCanHavePaymentPlan(payment);

            // Assert
            sut.IsValid().Should().BeFalse();
        }
    }
}