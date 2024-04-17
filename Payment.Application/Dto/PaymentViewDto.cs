using Payment.Application.Dto.Source;
using Payment.Domain.Constants;

namespace Payment.Application.Dto
{
    public class PaymentViewDto
    {
        public PaymentViewDto(
        Guid id,
        PaymentStatus status,
        string? cardNumber,
        PaymentType type,
        int amount,
        string currency,
        string merchantId,
        string reference,
        string? response,
        DateTimeOffset createdDateTime,
        DateTimeOffset lastUpdatedDateTime
        ) : this()
        {
            Id = id;
            Status = status;
            Source = new MaskedPaymentSourceDto(cardNumber);
            Type = type;
            Amount = amount;
            Currency = currency;
            MerchantId = merchantId;
            Reference = reference;
            Response = response;
            CreatedDateTime = createdDateTime;
            LastChangedDateTime = lastUpdatedDateTime;
        }

        private PaymentViewDto()
        {

        }

        public Guid Id { get; private set; }
        public PaymentStatus Status { get; private set; }
        public MaskedPaymentSourceDto Source { get; private set; }
        public PaymentType Type { get; private set; }
        public int Amount { get; private set; }
        public string Currency { get; private set; }
        public string MerchantId { get; private set; }
        public string Reference { get; private set; }
        public string? Response { get; private set; }
        public DateTimeOffset CreatedDateTime { get; private set; }
        public DateTimeOffset LastChangedDateTime { get; private set; }
    }
}
