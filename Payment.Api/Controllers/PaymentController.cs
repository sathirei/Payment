using IdempotentAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Payment.Application.Dto;
using Payment.Application.Services;

namespace Payment.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger) : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;
        private readonly ILogger<PaymentController> _logger = logger;

        [HttpPost]
        [Route("Payments")]
        [Idempotent(UseIdempotencyOption = true)]
        public async Task<IActionResult> PostAsync([FromBody] PaymentDto payment)
        {
            StringValues requestId = default;
            Request?.Headers?.TryGetValue("IdempotencyKey", out requestId);
            _logger.LogInformation("Payment requested: RequestId {RequestId}", requestId);

            var paymentResult = await _paymentService.PayAsync(payment);
            _logger.LogInformation("Payment accepted: {CorrelationId}", paymentResult.Id);
            return Accepted(paymentResult);
        }

        [HttpGet]
        [Route("Payments/{paymentId}")]

        public async Task<IActionResult> GetAsync([FromRoute] Guid paymentId)
        {
            _logger.LogInformation("Getting payment: {CorrelationId}", paymentId);
            var payment = await _paymentService.GetAsync(paymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found: {CorrelationId}", paymentId);
                return NotFound();
            }
            return Ok(payment);
        }
    }
}
