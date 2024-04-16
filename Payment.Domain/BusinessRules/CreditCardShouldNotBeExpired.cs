using Payment.Domain.Source;

namespace Payment.Domain.BusinessRules
{
    public class CreditCardShouldNotBeExpired(CreditCardPaymentSource source) : IBusinessRule
    {
        private readonly CreditCardPaymentSource _source = source;

        public string? BrokenRuleMessage => "Credit card has already expired.";

        public string Code => "credit-card-expired";

        public bool IsValid()
        {
            return _source != null
                && (_source.ExpiryYear > SystemTime.OffsetNow().Year
                || SameYearWithExpiryOnSameOrFutureMonths());
        }

        private bool SameYearWithExpiryOnSameOrFutureMonths() => _source.ExpiryYear == SystemTime.OffsetNow().Year
                            && _source.ExpiryMonth >= SystemTime.OffsetNow().Month;
    }
}
