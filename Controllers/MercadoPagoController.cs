using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TheBestBean.Data;
using TheBestBean.Services;

namespace TheBestBean.Controllers
{
    [IgnoreAntiforgeryToken]
    [ApiController]
    [Route("api/mercadopago")]
    public class MercadoPagoController : ControllerBase
    {
        private readonly MercadoPagoService _mp;
        private readonly TheBestBeanContext _db;
        private readonly ILogger<MercadoPagoController> _logger;

        public MercadoPagoController(MercadoPagoService mp, TheBestBeanContext db, ILogger<MercadoPagoController> logger)
        {
            _mp = mp;
            _db = db;
            _logger = logger;
        }

        [HttpPost("webhook")]
        [HttpGet("webhook")]
        public async Task<IActionResult> Webhook(CancellationToken ct)
        {
            var paymentId = Request.Query["data.id"].FirstOrDefault()
                ?? Request.Query["id"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(paymentId) && Request.ContentLength > 0)
            {
                using var doc = await JsonDocument.ParseAsync(Request.Body, cancellationToken: ct);
                if (doc.RootElement.TryGetProperty("data", out var data) && data.TryGetProperty("id", out var idEl))
                {
                    paymentId = idEl.ToString();
                }
            }

            if (string.IsNullOrWhiteSpace(paymentId) || paymentId == "0")
            {
                return Ok();
            }

            var payment = await _mp.GetPaymentAsync(paymentId, ct);
            if (payment == null || string.IsNullOrWhiteSpace(payment.ExternalReference))
            {
                _logger.LogWarning("Mercado Pago webhook payment {Id} not found", paymentId);
                return Ok();
            }

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderNumber == payment.ExternalReference, ct);
            if (order == null)
            {
                return Ok();
            }

            order.MercadoPagoPaymentId = payment.Id;
            if (payment.IsApproved)
            {
                order.PaymentStatus = "Paid";
            }
            else if (string.Equals(payment.Status, "rejected", StringComparison.OrdinalIgnoreCase)
                || string.Equals(payment.Status, "cancelled", StringComparison.OrdinalIgnoreCase))
            {
                order.PaymentStatus = "Failed";
            }

            await _db.SaveChangesAsync(ct);
            return Ok();
        }
    }
}
