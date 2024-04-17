using Payment.Domain.Constants;
using Payment.Domain.Source;

namespace Payment.Domain
{
    public class Payment : IEntityMarker
    {
        public Payment(
        Guid id,
        PaymentStatus status,
        PaymentSource source,
        PaymentType type,
        PaymentPlan? plan,
        int amount,
        string currency,
        string merchantId,
        string reference
        ) : this()
        {
            Id = id;
            Status = status;
            Source = source;
            Type = type;
            Plan = plan;
            Amount = amount;
            Currency = currency;
            MerchantId = merchantId;
            Reference = reference;
        }
        private Payment()
        {

        }
        public Guid Id { get; private set; }
        public PaymentStatus Status { get; private set; }
        public virtual PaymentSource Source { get; private set; }
        public PaymentType Type { get; private set; }
        public virtual PaymentPlan? Plan { get; private set; }
        public int Amount { get; private set; }
        public string Currency { get; private set; }
        public string MerchantId { get; private set; }
        public string Reference { get; private set; }
        public string? Response { get; private set; }
        public DateTimeOffset CreatedDateTime { get; private set; }
        public DateTimeOffset LastChangedDateTime { get; private set; }

        public void UpdateResponse(string? response)
        {
            Response = response;
        }

        public void UpdateStatus(PaymentStatus status)
        {
            Status = status;
        }
    }
}
