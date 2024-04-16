namespace Payment.Application.Dto
{
    public class PaymentPlanDto
    {
        public int DaysBetweenPayments { get; set; }
        public int TotalNumberOfPayments { get; set; }
        public int CurrentPaymentNumber { get; set; }
        public DateTime Expiry { get; set; }
    }
}
