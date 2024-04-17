namespace Payment.Event
{
    public class PaymentEvent
    {
        public Guid Id { get; set; }
        public EventType EventType { get; set; }

        public object Payload { get; set; }
    }
}
