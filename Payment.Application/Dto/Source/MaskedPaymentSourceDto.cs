namespace Payment.Application.Dto.Source
{
    public class MaskedPaymentSourceDto(string? cardNumber)
    {
        public string? MaskedCardNumber { get; private set; } = cardNumber;
    }
}
