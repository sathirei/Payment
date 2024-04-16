using Payment.Domain.Constants;

namespace Payment.Domain.Source
{
    public abstract class PaymentSource(PaymentSourceType type)
    {
        public int Id { get; private set; }
        public PaymentSourceType Type { get; private set; } = type;
    }
}
