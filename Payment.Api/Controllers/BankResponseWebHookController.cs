using IdempotentAPI.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Payment.Domain;
using Payment.Event;

namespace Payment.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class BankResponseWebHookController(
        IEventProducer<PaymentEvent> paymentEventProducer,
        ILogger<BankResponseWebHookController> logger) : ControllerBase
    {
        private readonly IEventProducer<PaymentEvent> _paymentEventProducer = paymentEventProducer;
        private readonly ILogger<BankResponseWebHookController> _logger = logger;

        [HttpPost]
        [Route("WebHook")]
        [Idempotent(UseIdempotencyOption = true)]
        public async Task<IActionResult> PostAsync([FromBody] BankResponse bankResponse)
        {
            StringValues requestId = default;
            Request?.Headers?.TryGetValue("IdempotencyKey", out requestId);
            _logger.LogInformation("Bank response received: RequestId {RequestId}, PaymentId {CorrelationId}", requestId, bankResponse.Id);
            // TODO: Use Inbox pattern instead
            await _paymentEventProducer.ProduceAsync(new PaymentEvent
            {
                Id = bankResponse.Id,
                EventType = EventType.ResponseFromBank,
                Payload = bankResponse
            });
            return Accepted();
        }
    }
}
