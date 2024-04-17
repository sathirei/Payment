namespace Payment.Event
{
    public interface IPaymentEventProcessor
    {
        public Task ProcessAsync(PaymentEvent paymentEvent);
    }
}
