using FluentAssertions;
using Payment.Domain.Constants;

namespace Payment.Domain.Tests
{
    public class PaymentTests
    {
        [Fact()]
        public void PaymentPlan_Should_SetFields()
        {
            // Arrange
            var id = Guid.NewGuid();
            var status = PaymentStatus.EXECUTING;
            var source = Fixture.NewCreditCardPaymentSource(2024, 12);
            var type = PaymentType.Recurring;
            var plan = Fixture.NewPaymentPlan();
            var amount = 10000;
            var currency = "GBP";
            var merchantId = "Amazon_001";
            var reference = "00123456abc";

            // Act
            var sut = new Payment(
                id,
                status,
                source,
                type,
                plan,
                amount,
                currency,
                merchantId,
                reference
                );

            // Assert
            sut.Id.Should().Be(id);
            sut.Status.Should().Be(status);
            sut.Source.Should().Be(source);
            sut.Type.Should().Be(type);
            sut.Plan.Should().Be(plan);
            sut.Amount.Should().Be(amount);
            sut.Currency.Should().Be(currency);
            sut.MerchantId.Should().Be(merchantId);
            sut.Reference.Should().Be(reference);
        }
    }
}
