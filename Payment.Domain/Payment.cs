using Payment.Domain.Constants;
using Payment.Domain.Source;

namespace Payment.Domain
{
    public class Payment(
        Guid id,
        PaymentStatus status,
        PaymentSource source,
        PaymentType type,
        PaymentPlan? plan,
        int amount,
        string currency,
        string merchantId,
        string reference
        )
    {
        public Guid Id { get; private set; } = id;
        public PaymentStatus Status { get; private set; } = status;
        public PaymentSource Source { get; private set; } = source;
        public PaymentType Type { get; private set; } = type;

        public PaymentPlan? Plan { get; private set; } = plan;

        public int Amount { get; private set; } = amount;
        public string Currency { get; private set; } = currency;
        public string MerchantId { get; private set; } = merchantId;
        public string Reference { get; private set; } = reference;
        public DateTimeOffset CreatedDateTime { get; private set; }
        public DateTimeOffset UpdateDateTime { get; private set; }
    }
}
