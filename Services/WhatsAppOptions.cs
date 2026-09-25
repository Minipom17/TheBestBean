namespace TheBestBean.Services;

/// <summary>
/// Ops WhatsApp alerts (checkout / bookings). Configure via secrets/whatsapp.json.
/// CallMeBot: text "I allow callmebot to send me messages" to +34 644 66 78 53,
/// then save the API key it replies with.
/// </summary>
public class WhatsAppOptions
{
    /// <summary>CallMeBot (default) or Meta (Cloud API).</summary>
    public string Provider { get; set; } = "CallMeBot";

    /// <summary>Digits only, e.g. 51913779574. Defaults to Purple Bean lab phone.</summary>
    public string NotifyDigits { get; set; } = PurpleBeanContact.WhatsAppDigits;

    /// <summary>CallMeBot API key from the bot reply.</summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Meta Cloud API permanent token.</summary>
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Meta Cloud API phone-number-id.</summary>
    public string PhoneNumberId { get; set; } = string.Empty;

    /// <summary>Optional ops email fallback when WhatsApp is not configured.</summary>
    public string NotifyEmail { get; set; } = string.Empty;

    public bool IsCallMeBot =>
        Provider.Equals("CallMeBot", StringComparison.OrdinalIgnoreCase);

    public bool IsMeta =>
        Provider.Equals("Meta", StringComparison.OrdinalIgnoreCase);

    public bool IsWhatsAppConfigured =>
        IsCallMeBot
            ? !string.IsNullOrWhiteSpace(ApiKey) && !string.IsNullOrWhiteSpace(NotifyDigits)
            : IsMeta
                && !string.IsNullOrWhiteSpace(AccessToken)
                && !string.IsNullOrWhiteSpace(PhoneNumberId)
                && !string.IsNullOrWhiteSpace(NotifyDigits);

    public bool IsEmailFallbackConfigured => !string.IsNullOrWhiteSpace(NotifyEmail);

    public bool IsConfigured => IsWhatsAppConfigured || IsEmailFallbackConfigured;
}
