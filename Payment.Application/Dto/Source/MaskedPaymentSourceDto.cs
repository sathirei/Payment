using Payment.Domain.Source;

namespace Payment.Application.Dto.Source
{
    public class MaskedPaymentSourceDto
    {
        public string? MaskedCardNumber { get; private set; }

        public MaskedPaymentSourceDto(string? cardNumber)
        {
            MaskedCardNumber = cardNumber;
        }
    }
}
