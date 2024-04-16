using Payment.Application.Dto;
using Payment.Application.Mapper;
using Payment.Infrastructure;
using Payment.Infrastructure.Persistence.Repositories;

namespace Payment.Application.Services
{
    public class PaymentService(IRepository<Domain.Payment> repository, IUnitOfWork unitOfWork) : IPaymentService
    {
        private readonly IRepository<Domain.Payment> _reposisotry = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<PaymentViewDto?> GetAsync(Guid paymentId)
        {
            var payment = await _reposisotry.FindByIdAsync(paymentId);
            return PaymentMapper.Map(payment: payment);
        }

        public async Task<PaymentResultDto> PayAsync(PaymentDto payment)
        {
            var paymentDomain = PaymentMapper.Map(payment);
            await _reposisotry.AddAsync(paymentDomain!);
            await _unitOfWork.CompleteAsync();
            return new PaymentResultDto
            {
                Id = paymentDomain!.Id,
                Status = paymentDomain.Status
            };
        }
    }
}
