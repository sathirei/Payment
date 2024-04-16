using Payment.Domain;
using Payment.Domain.BusinessRules;

namespace Payment.Application.BusinessRules
{
    public class CreditCardShouldNotBeExpired(int? year, int? month) : IBusinessRule
    {
        public string? BrokenRuleMessage => "Credit card has already expired.";

        public string Code => "credit-card-expired";

        public bool IsValid()
        {
            return year > SystemTime.OffsetNow().Year
                || SameYearWithExpiryOnSameOrFutureMonths(year, month);
        }

        private bool SameYearWithExpiryOnSameOrFutureMonths(int? year, int? month) => year == SystemTime.OffsetNow().Year
                            && month >= SystemTime.OffsetNow().Month;
    }
}
