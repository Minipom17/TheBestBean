using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace TheBestBean.Services
{
    public class CulqiService
    {
        private readonly HttpClient _http;
        private readonly CulqiOptions _options;
        private readonly ILogger<CulqiService> _logger;

        public CulqiService(HttpClient http, CulqiOptions options, ILogger<CulqiService> logger)
        {
            _http = http;
            _options = options;
            _logger = logger;
            _http.BaseAddress = new Uri("https://api.culqi.com/v2/");
        }

        public bool IsConfigured => _options.IsConfigured;
        public string PublicKey => _options.PublicKey;
        public string RsaId => _options.RsaId;
        public string RsaPublicKey => _options.RsaPublicKey;

        public static bool IsCulqiMethod(string? paymentMethod) =>
            string.Equals(paymentMethod, "Culqi", StringComparison.OrdinalIgnoreCase);

        public async Task<(bool Ok, string ChargeId, string Error)> ChargeAsync(
            string sourceId,
            int amountCentimos,
            string email,
            string orderNumber,
            CancellationToken ct = default)
        {
            if (!IsConfigured)
            {
                return (false, "", "Culqi is not connected yet.");
            }

            if (string.IsNullOrWhiteSpace(sourceId) || amountCentimos < 100)
            {
                return (false, "", "Culqi token was missing. Try the card form again.");
            }

            var payload = new Dictionary<string, object?>
            {
                ["amount"] = amountCentimos,
                ["currency_code"] = "PEN",
                ["email"] = email,
                ["source_id"] = sourceId,
                ["description"] = $"Purple Bean {orderNumber}",
                ["metadata"] = new Dictionary<string, string> { ["order"] = orderNumber }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "charges")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.SecretKey);

            using var response = await _http.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Culqi charge failed {Status}: {Body}", (int)response.StatusCode, body);
                return (false, "", UserMessage(body));
            }

            using var doc = JsonDocument.Parse(body);
            var id = doc.RootElement.TryGetProperty("id", out var idEl) ? idEl.GetString() ?? "" : "";
            var outcome = doc.RootElement.TryGetProperty("outcome", out var outEl)
                && outEl.TryGetProperty("type", out var typeEl)
                    ? typeEl.GetString()
                    : "";
            if (!string.IsNullOrEmpty(outcome) &&
                !string.Equals(outcome, "venta_exitosa", StringComparison.OrdinalIgnoreCase))
            {
                return (false, id, UserMessage(body));
            }

            return (true, id, "");
        }

        private static string UserMessage(string body)
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("user_message", out var msg))
                {
                    var text = msg.GetString();
                    if (!string.IsNullOrWhiteSpace(text)) return text;
                }
                if (doc.RootElement.TryGetProperty("merchant_message", out var merch))
                {
                    var text = merch.GetString();
                    if (!string.IsNullOrWhiteSpace(text)) return text;
                }
            }
            catch
            {
                // fall through
            }

            return "Culqi could not charge the card. Try Yape or another card.";
        }
    }
}
