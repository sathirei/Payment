using Payment.Application.Dto;
using Payment.Application.Dto.Source;
using Payment.Domain.Constants;
using Payment.Domain.Source;

namespace Payment.Application.Mapper
{
    public static class PaymentMapper
    {
        public static Domain.Payment? Map(PaymentDto? payment)
        {
            if (payment == null)
            {
                return null;
            }

            return new Domain.Payment(
                Guid.NewGuid(),
                PaymentStatus.EXECUTING,
                MapSource(payment.Source),
                payment.Type,
                null,
                payment.Amount,
                payment.Currency,
                payment.MerchantId,
                payment.Reference
                );
        }

        public static PaymentViewDto? Map(Domain.Payment? payment)
        {
            if (payment == null)
            {
                return null;
            }
            var cardNumber = GetCardNumber(payment.Source);
            return new PaymentViewDto(
                payment.Id,
                payment.Status,
                cardNumber?.MaskedLast('x', 4),
                payment.Type,
                payment.Amount,
                payment.Currency,
                payment.MerchantId,
                payment.Reference,
                payment.Response,
                payment.CreatedDateTime,
                payment.LastChangedDateTime);
        }

        private static string? GetCardNumber(PaymentSource source)
        {
            if (source.Type == PaymentSourceType.CreditCard)
            {
                return ((CreditCardPaymentSource)source).Number;
            }
            return null;
        }

        private static PaymentSource MapSource(PaymentSourceDto source)
        {
            return source?.Type switch
            {
                PaymentSourceType.CreditCard => new CreditCardPaymentSource(
                    number: source.Number,
                    expiryMonth: source.ExpiryMonth,
                    expiryYear: source.ExpiryYear,
                    name: source.Name),
                _ => throw new ArgumentException("Unsupported payment source")
            };
        }
    }
}
