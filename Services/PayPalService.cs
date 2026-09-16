using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using TheBestBean.Models;

namespace TheBestBean.Services
{
    public class PayPalService
    {
        private readonly HttpClient _http;
        private readonly PayPalOptions _options;
        private readonly IMemoryCache _cache;
        private readonly ILogger<PayPalService> _logger;

        public PayPalService(HttpClient http, PayPalOptions options, IMemoryCache cache, ILogger<PayPalService> logger)
        {
            _http = http;
            _options = options;
            _cache = cache;
            _logger = logger;
            _http.BaseAddress = new Uri(_options.ApiBase.TrimEnd('/') + "/");
        }

        public bool IsConfigured => _options.IsConfigured;

        public async Task<string?> CreateCheckoutUrlAsync(Order order, decimal cadAmount, string publicBaseUrl, CancellationToken ct = default)
        {
            if (!IsConfigured) return null;

            var token = await GetAccessTokenAsync(ct);
            if (token == null) return null;

            var back = publicBaseUrl.TrimEnd('/');
            var cad = cadAmount.ToString("0.00", CultureInfo.InvariantCulture);
            var payload = new
            {
                intent = "CAPTURE",
                purchase_units = new[]
                {
                    new
                    {
                        reference_id = order.OrderNumber,
                        custom_id = order.OrderNumber,
                        description = $"Purple Bean {order.OrderNumber}",
                        amount = new
                        {
                            currency_code = "CAD",
                            value = cad
                        }
                    }
                },
                application_context = new
                {
                    brand_name = "Purple Bean Coffee",
                    user_action = "PAY_NOW",
                    return_url = $"{back}/PayPalComplete?order={Uri.EscapeDataString(order.OrderNumber)}",
                    cancel_url = $"{back}/Checkout?paypal=cancel"
                }
            };

            using var request = new HttpRequestMessage(HttpMethod.Post, "v2/checkout/orders")
            {
                Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _http.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("PayPal create order failed {Status}: {Body}", (int)response.StatusCode, body);
                return null;
            }

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            if (root.TryGetProperty("id", out var id))
            {
                order.PayPalOrderId = id.GetString() ?? string.Empty;
            }

            if (root.TryGetProperty("links", out var links))
            {
                foreach (var link in links.EnumerateArray())
                {
                    var rel = link.TryGetProperty("rel", out var relEl) ? relEl.GetString() : "";
                    if (rel is "payer-action" or "approve")
                    {
                        return link.GetProperty("href").GetString();
                    }
                }
            }

            return null;
        }

        public async Task<bool> CaptureAsync(string payPalOrderId, CancellationToken ct = default)
        {
            if (!IsConfigured || string.IsNullOrWhiteSpace(payPalOrderId)) return false;
            var token = await GetAccessTokenAsync(ct);
            if (token == null) return false;

            using var request = new HttpRequestMessage(HttpMethod.Post, $"v2/checkout/orders/{Uri.EscapeDataString(payPalOrderId)}/capture")
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _http.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            if (body.Contains("ORDER_ALREADY_CAPTURED", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("PayPal capture failed {Status}: {Body}", (int)response.StatusCode, body);
                return false;
            }

            using var doc = JsonDocument.Parse(body);
            var status = doc.RootElement.TryGetProperty("status", out var st) ? st.GetString() : "";
            return status is "COMPLETED" or "APPROVED";
        }

        private async Task<string?> GetAccessTokenAsync(CancellationToken ct)
        {
            if (_cache.TryGetValue("paypal_access_token", out string? cached) && !string.IsNullOrEmpty(cached))
            {
                return cached;
            }

            var raw = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.ClientId}:{_options.Secret}"));
            using var request = new HttpRequestMessage(HttpMethod.Post, "v1/oauth2/token");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", raw);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            });

            using var response = await _http.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("PayPal token failed {Status}: {Body}", (int)response.StatusCode, body);
                return null;
            }

            using var doc = JsonDocument.Parse(body);
            var access = doc.RootElement.GetProperty("access_token").GetString();
            var expires = doc.RootElement.TryGetProperty("expires_in", out var exp) ? exp.GetInt32() : 300;
            if (!string.IsNullOrEmpty(access))
            {
                _cache.Set("paypal_access_token", access, TimeSpan.FromSeconds(Math.Max(60, expires - 60)));
            }
            return access;
        }
    }
}
