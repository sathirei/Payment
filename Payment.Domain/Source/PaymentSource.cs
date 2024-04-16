using Payment.Domain.Constants;

namespace Payment.Domain.Source
{
    public abstract class PaymentSource(PaymentSourceType type)
    {
        public PaymentSourceType Type { get; private set; } = type;
    }
}
