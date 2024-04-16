namespace Payment.Infrastructure.Persistence.Repositories
{
    public class PaymentRepository(PaymentContext context)
        : Repository<Domain.Payment>(context), IPaymentRepository
    {
    }
}
