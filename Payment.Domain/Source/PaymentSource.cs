using Payment.Domain.Constants;

namespace Payment.Domain.Source
{
    public abstract class PaymentSource(PaymentSourceType type)
    {
        public int Id { get; private set; }
        public PaymentSourceType Type { get; private set; } = type;

        public virtual Payment Payment { get; private set; }
        public Guid PaymentId { get; set; }
    }
}
