using Payment.Domain.Constants;

namespace Payment.Domain.Source
{
    public class CreditCardPaymentSource(
        string number,
        int expiryMonth,
        int expiryYear,
        string name) : PaymentSource(PaymentSourceType.CreditCard)
    {
        public string Number { get; private set; } = number;
        public int ExpiryMonth { get; private set; } = expiryMonth;
        public int ExpiryYear { get; private set; } = expiryYear;
        public string Name { get; private set; } = name;
    }
}
