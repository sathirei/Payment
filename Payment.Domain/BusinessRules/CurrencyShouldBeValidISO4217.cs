using ISO._4217;

namespace Payment.Domain.BusinessRules
{
    public class CurrencyShouldBeValidISO4217(string isoCurrencyCode) : IBusinessRule
    {
        public string? BrokenRuleMessage => "Currency should be a valid ISO 4217 code.";

        public string Code => "currency-should-be-valid-iso-4217";

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(isoCurrencyCode) && CurrencyCodesResolver.Codes                
                .Exists(currency => currency.Code == isoCurrencyCode);
        }
    }
}
