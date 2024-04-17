namespace Payment.Event
{
    public class PaymentEvent
    {
        public Guid Id { get; set; }
        public EventType EventType { get; set; }

        public string? Payload { get; set; }
    }
}
