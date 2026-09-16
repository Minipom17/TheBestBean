using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TheBestBean.Models;

namespace TheBestBean.Services
{
    public class MercadoPagoService
    {
        private static readonly JsonSerializerOptions JsonOpts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _http;
        private readonly MercadoPagoOptions _options;
        private readonly ILogger<MercadoPagoService> _logger;

        public MercadoPagoService(HttpClient http, MercadoPagoOptions options, ILogger<MercadoPagoService> logger)
        {
            _http = http;
            _options = options;
            _logger = logger;
            _http.BaseAddress = new Uri("https://api.mercadopago.com/");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        }

        public bool IsConfigured => _options.IsConfigured;

        public async Task<string?> CreateCheckoutUrlAsync(Order order, decimal penAmount, string publicBaseUrl, CancellationToken ct = default)
        {
            if (!IsConfigured) return null;

            var back = publicBaseUrl.TrimEnd('/');
            var amount = Math.Round(penAmount, 2).ToString("0.00");
            var payload = new
            {
                type = "online",
                processing_mode = "manual",
                capture_mode = "automatic_async",
                total_amount = amount,
                external_reference = order.OrderNumber,
                expiration_time = "P1D",
                payer = new { email = order.Email, name = order.FullName },
                items = new[]
                {
                    new
                    {
                        title = $"Purple Bean {order.OrderNumber}",
                        unit_price = amount,
                        quantity = 1,
                        unit_measure = "unit",
                        total_amount = amount
                    }
                },
                config = new
                {
                    online = new
                    {
                        success_url = $"{back}/OrderConfirmation?order={Uri.EscapeDataString(order.OrderNumber)}",
                        failure_url = $"{back}/Checkout?mp=failure",
                        pending_url = $"{back}/OrderConfirmation?order={Uri.EscapeDataString(order.OrderNumber)}"
                    }
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "v1/orders")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload, JsonOpts), Encoding.UTF8, "application/json")
            };
            request.Headers.TryAddWithoutValidation("X-Idempotency-Key", Guid.NewGuid().ToString());

            using var response = await _http.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Mercado Pago order failed {Status}: {Body}", (int)response.StatusCode, body);
                return null;
            }

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.TryGetProperty("id", out var id))
            {
                order.MercadoPagoPreferenceId = id.GetString() ?? string.Empty;
            }

            return root.TryGetProperty("checkout_url", out var url) ? url.GetString() : null;
        }

        public async Task<MercadoPagoPayment?> GetPaymentAsync(string paymentId, CancellationToken ct = default)
        {
            if (!IsConfigured || string.IsNullOrWhiteSpace(paymentId)) return null;

            var path = paymentId.StartsWith("ORD", StringComparison.OrdinalIgnoreCase)
                ? $"v1/orders/{Uri.EscapeDataString(paymentId)}"
                : $"v1/payments/{Uri.EscapeDataString(paymentId)}";

            using var response = await _http.GetAsync(path, ct);
            if (!response.IsSuccessStatusCode)
            {
                using var fallback = await _http.GetAsync($"v1/orders/{Uri.EscapeDataString(paymentId)}", ct);
                if (!fallback.IsSuccessStatusCode) return null;
                return ParseStatus(await fallback.Content.ReadAsStringAsync(ct), paymentId);
            }

            return ParseStatus(await response.Content.ReadAsStringAsync(ct), paymentId);
        }

        private static MercadoPagoPayment ParseStatus(string body, string fallbackId)
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            return new MercadoPagoPayment
            {
                Id = root.TryGetProperty("id", out var id) ? id.ToString() : fallbackId,
                Status = root.TryGetProperty("status", out var status) ? status.GetString() ?? string.Empty : string.Empty,
                ExternalReference = root.TryGetProperty("external_reference", out var ext) ? ext.GetString() ?? string.Empty : string.Empty
            };
        }
    }

    public class MercadoPagoPayment
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ExternalReference { get; set; } = string.Empty;
        public bool IsApproved =>
            Status is "approved" or "processed" or "accredited" or "paid";
    }
}
