using Payment.Application.Dto.Source;
using Payment.Domain.Constants;

namespace Payment.Application.Dto
{
    public class PaymentDto
    {
        public PaymentSourceDto Source { get; set; }
        public PaymentType Type { get; set; }
        public int Amount { get; set; }
        public string Currency { get; set; }
        public string MerchantId { get; set; }
        public string Reference { get; set; }
        public DateTimeOffset CreatedDateTime { get; set; }
        public DateTimeOffset LastChangedDateTime { get; set; }
    }
}
