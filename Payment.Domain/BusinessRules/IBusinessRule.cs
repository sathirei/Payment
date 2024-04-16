namespace Payment.Domain.BusinessRules
{
    public interface IBusinessRule
    {
        string? BrokenRuleMessage { get; }

        string Code { get; }

        bool IsValid();
    }
}
