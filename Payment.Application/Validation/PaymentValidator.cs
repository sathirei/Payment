using FluentValidation;
using Payment.Application.BusinessRules;
using Payment.Application.Dto;
using Payment.Application.Dto.Source;
using Payment.Domain.Constants;

namespace Payment.Application.Validation
{
    public class PaymentValidator : AbstractValidator<PaymentDto>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.Source).NotNull();
            RuleFor(x => x.Amount).Must(x => x > 0)
                .WithMessage("Payment amount must be greater than 0.");

            RuleFor(x => x.MerchantId).Must(NotNullOrEmpty)
                .WithMessage("'MerchantId' should not be null or empty.");
            RuleFor(x => x.Reference).Must(NotNullOrEmpty)
                .WithMessage("'Reference' should not be null or empty.");

            RuleFor(x => x.Type).Must(IsSupported)
                .WithMessage("Payment type is currently not supported.");

            RuleFor(x => x.Source)
                .Must(BeAValidMonth)
                .WithMessage("Expiry month should be between 1 and 12 (January to December).")
                .OverridePropertyName("Source.ExpiryMonth");

            RuleFor(x => x.Source)
                .Must(BeFourDigits)
                .WithMessage("Year should be in 4 digit format e.g. 1999.")
                .OverridePropertyName("Source.ExpiryYear");

            RuleFor(x => x.Currency)
                .MustFollowBusinessRule(c =>
                new CurrencyShouldBeValidISO4217(c));
            RuleFor(x => x.Source)
                .MustFollowBusinessRule(s =>
                new CreditCardShouldNotBeExpired(s?.ExpiryYear, s?.ExpiryMonth));
        }

        private bool BeFourDigits(PaymentSourceDto source)
        {
            return source?.ExpiryYear.ToString().Length == 4;
        }

        private bool BeAValidMonth(PaymentSourceDto source)
        {
            return source != null && source.ExpiryMonth > 0 && source.ExpiryMonth < 13;
        }

        private bool NotNullOrEmpty(string value)
        {
            return !string.IsNullOrEmpty(value);
        }

        private bool IsSupported(PaymentType type)
        {
            return type != PaymentType.Recurring;
        }
    }
}
