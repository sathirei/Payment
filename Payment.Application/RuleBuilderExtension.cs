using FluentValidation;
using Payment.Domain.BusinessRules;

namespace Payment.Application
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> MustFollowBusinessRule<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, Func<TProperty, IBusinessRule> ruleSelector)
        {
            if (ruleSelector == null)
            {
                throw new ArgumentNullException("ruleSelector");
            }

            return ruleBuilder.Must((T x, TProperty val) => ruleSelector(val).IsValid()).WithMessage((T x, TProperty val) => ruleSelector(val).BrokenRuleMessage);
        }
    }
}
