using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace TheBestBean.Services;

public class WhatsAppNotifyService
{
    private readonly HttpClient _http;
    private readonly WhatsAppOptions _options;
    private readonly IEmailSender _email;
    private readonly ILogger<WhatsAppNotifyService> _logger;

    public WhatsAppNotifyService(
        HttpClient http,
        WhatsAppOptions options,
        IEmailSender email,
        ILogger<WhatsAppNotifyService> logger)
    {
        _http = http;
        _options = options;
        _email = email;
        _logger = logger;
    }

    public bool IsConfigured => _options.IsConfigured;

    public async Task<bool> SendOpsAsync(string message, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return false;
        }

        var ok = false;
        if (_options.IsWhatsAppConfigured)
        {
            ok = _options.IsMeta
                ? await SendMetaAsync(_options.NotifyDigits, message, ct)
                : await SendCallMeBotAsync(_options.NotifyDigits, message, ct);
        }
        else
        {
            _logger.LogWarning(
                "WhatsApp ops alerts not configured (secrets/whatsapp.json). Message: {Preview}",
                Preview(message));
        }

        if (_options.IsEmailFallbackConfigured)
        {
            try
            {
                await _email.SendEmailAsync(
                    _options.NotifyEmail,
                    "Purple Bean booking alert",
                    "<pre style=\"font-family:monospace;white-space:pre-wrap\">"
                        + WebUtility.HtmlEncode(message)
                        + "</pre>");
                ok = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ops email fallback failed.");
            }
        }

        return ok;
    }

    private async Task<bool> SendCallMeBotAsync(string digits, string message, CancellationToken ct)
    {
        var url =
            "https://api.callmebot.com/whatsapp.php"
            + "?phone=" + Uri.EscapeDataString(DigitsOnly(digits))
            + "&text=" + Uri.EscapeDataString(message)
            + "&apikey=" + Uri.EscapeDataString(_options.ApiKey);

        try
        {
            using var response = await _http.GetAsync(url, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("CallMeBot failed {Status}: {Body}", (int)response.StatusCode, body);
                return false;
            }

            _logger.LogInformation("CallMeBot ops alert sent ({Chars} chars).", message.Length);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CallMeBot request failed.");
            return false;
        }
    }

    private async Task<bool> SendMetaAsync(string digits, string message, CancellationToken ct)
    {
        var url = $"https://graph.facebook.com/v20.0/{_options.PhoneNumberId}/messages";
        var payload = new Dictionary<string, object?>
        {
            ["messaging_product"] = "whatsapp",
            ["to"] = DigitsOnly(digits),
            ["type"] = "text",
            ["text"] = new Dictionary<string, object?>
            {
                ["preview_url"] = false,
                ["body"] = message.Length > 4000 ? message[..4000] : message
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json")
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);

        try
        {
            using var response = await _http.SendAsync(request, ct);
            var body = await response.Content.ReadAsStringAsync(ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Meta WhatsApp failed {Status}: {Body}", (int)response.StatusCode, body);
                return false;
            }

            _logger.LogInformation("Meta WhatsApp ops alert sent ({Chars} chars).", message.Length);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Meta WhatsApp request failed.");
            return false;
        }
    }

    private static string DigitsOnly(string value) =>
        new string((value ?? "").Where(char.IsDigit).ToArray());

    private static string Preview(string message) =>
        message.Length <= 160 ? message : message[..157] + "…";
}
