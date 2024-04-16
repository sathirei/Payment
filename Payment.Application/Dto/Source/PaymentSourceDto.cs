using Payment.Domain.Constants;

namespace Payment.Application.Dto.Source
{
    public class PaymentSourceDto
    {
        public PaymentSourceType Type { get; set; }

        public string Number { get; set; }
        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }
        public string Name { get; set; }
    }
}
