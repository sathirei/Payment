using Payment.Domain.Constants;

namespace Payment.Application.Dto
{
    [Serializable]
    public class PaymentResultDto
    {
        public Guid Id { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
