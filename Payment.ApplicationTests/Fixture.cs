using Payment.Domain;
using Payment.Domain.Constants;
using Payment.Domain.Source;

namespace Payment.Application.Tests
{
    public static class Fixture
    {
        public static Domain.Payment NewPayment(PaymentType type, bool withPlan = false) => new(
            Guid.NewGuid(),
            PaymentStatus.EXECUTING,
            NewCreditCardPaymentSource(2024, 12),
            type,
            withPlan ? NewPaymentPlan() : null,
            10000,
            "GBP",
            "Amazon_001",
            "00123456abc"
            );

        public static CreditCardPaymentSource NewCreditCardPaymentSource(int expiryYear, int expiryMonth) => new(
            "1111222233334444",
            expiryMonth,
            expiryYear,
            "John Doe");

        public static PaymentPlan NewPaymentPlan() => new(
            30,
            6,
            1,
            new DateTime(2024, 10, 31)
            );
    }
}
