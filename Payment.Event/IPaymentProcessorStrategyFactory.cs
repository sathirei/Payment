namespace Payment.Event
{
    public interface IPaymentProcessorStrategyFactory
    {
        public IPaymentEventProcessor CreateStrategy(EventType eventType);
    }
}
