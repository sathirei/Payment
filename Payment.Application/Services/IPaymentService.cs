using Payment.Application.Dto;

namespace Payment.Application.Services
{
    public interface IPaymentService
    {
        public Task<PaymentResultDto> PayAsync(PaymentDto payment);
        public Task<PaymentViewDto?> GetAsync(Guid paymentId);
    }
}
