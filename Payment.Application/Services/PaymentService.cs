using Payment.Application.Dto;
using Payment.Application.Mapper;
using Payment.Event;
using Payment.Infrastructure;
using Payment.Infrastructure.Persistence.Repositories;

namespace Payment.Application.Services
{
    public class PaymentService(
        IRepository<Domain.Payment> repository,
        IUnitOfWork unitOfWork,
        IEventProducer<PaymentEvent> paymentEventProducer) : IPaymentService
    {
        private readonly IRepository<Domain.Payment> _reposisotry = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IEventProducer<PaymentEvent> _paymentEventProducer = paymentEventProducer;

        public async Task<PaymentViewDto?> GetAsync(Guid paymentId)
        {
            var payment = await _reposisotry.FindByIdAsync(paymentId);
            return PaymentMapper.Map(payment: payment);
        }

        public async Task<PaymentResultDto> PayAsync(PaymentDto payment)
        {
            var paymentDomain = PaymentMapper.Map(payment);
            await _reposisotry.AddAsync(paymentDomain!);

            // TODO: Use Outbox pattern instead
            await _paymentEventProducer.ProduceAsync(new PaymentEvent
            {
                EventType = EventType.SendToBank,
                Id = paymentDomain!.Id,
                Payload = "Dummy" //TODO: Use payload as per Bank API Specification
            });

            await _unitOfWork.CompleteAsync();
            return new PaymentResultDto
            {
                Id = paymentDomain!.Id,
                Status = paymentDomain.Status
            };
        }
    }
}
