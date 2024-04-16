namespace Payment.Infrastructure
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
