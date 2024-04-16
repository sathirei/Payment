using Payment.Domain.Constants;

namespace Payment.Domain.BusinessRules
{
    public class OnlyRecurringPaymentCanHavePaymentPlan(Payment payment) : IBusinessRule
    {
        private readonly Payment _payment = payment;

        public string? BrokenRuleMessage =>
            "Payment plan can only be associated with 'Recurring' payment type.";

        public string Code => "payment-plan-only-valid-for-recurring-payment-type";

        public bool IsValid()
        {
            return IsNotRecurringPaymentType()
                || IsRecurringPaymentTypeWithPlan();
        }

        private bool IsRecurringPaymentTypeWithPlan()
            => _payment?.Type == PaymentType.Recurring && _payment.Plan != null;

        private bool IsNotRecurringPaymentType()
            => _payment?.Type != PaymentType.Recurring && _payment?.Plan == null;
    }
}
