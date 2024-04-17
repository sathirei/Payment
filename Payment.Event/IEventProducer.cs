namespace Payment.Event
{
    public interface IEventProducer<T>
    {
        public Task ProduceAsync(T message);
    }
}
