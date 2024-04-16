namespace Payment.Domain
{
    public class PaymentPlan(
        int daysBetweenPayments,
        int totalNumberOfPayments,
        int currentPaymentNumber,
        DateTime expiry)
    {
        public int DaysBetweenPayments { get; private set; } = daysBetweenPayments;
        public int TotalNumberOfPayments { get; private set; } = totalNumberOfPayments;
        public int CurrentPaymentNumber { get; private set; } = currentPaymentNumber;
        public DateTime Expiry { get; private set; } = expiry;
    }
}
