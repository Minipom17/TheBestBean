using TheBestBean.Models;

namespace TheBestBean.Services;

/// <summary>Builds and sends ops WhatsApp alerts for experience checkout / bookings.</summary>
public class BookingOpsNotifyService
{
    private readonly WhatsAppNotifyService _whatsApp;
    private readonly ILogger<BookingOpsNotifyService> _logger;

    public BookingOpsNotifyService(WhatsAppNotifyService whatsApp, ILogger<BookingOpsNotifyService> logger)
    {
        _whatsApp = whatsApp;
        _logger = logger;
    }

    public bool IsConfigured => _whatsApp.IsConfigured;

    public Task NotifyCheckoutVisitAsync(
        string? name,
        string? phone,
        string? email,
        IReadOnlyList<CartItem> cart,
        CancellationToken ct = default)
    {
        var experienceLines = cart.Where(i => IsExperience(i.ProductType)).ToList();
        if (experienceLines.Count == 0)
        {
            return Task.CompletedTask;
        }

        var tourist = LooksLikeTourist(phone, paymentMethod: null);
        var lines = new List<string>
        {
            tourist ? "🚨 TOURIST on CHECKOUT" : "👀 Someone on CHECKOUT",
            "They have not paid yet — open if you can take the booking.",
            ""
        };
        AppendCartLines(lines, experienceLines);
        AppendContact(lines, name, phone, email);
        return FireAndForgetAsync(string.Join('\n', lines), ct);
    }

    public Task NotifyOrderPlacedAsync(Order order, CancellationToken ct = default)
    {
        var experienceLines = order.Items.Where(i => IsExperience(i.ProductType)).ToList();
        if (experienceLines.Count == 0)
        {
            return Task.CompletedTask;
        }

        var tourist = LooksLikeTourist(order.Phone, order.PaymentMethod);
        var paid = string.Equals(order.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase);
        var lines = new List<string>
        {
            tourist
                ? (paid ? "🚨 TOURIST PAID — BOOKING" : "🚨 TOURIST clicked PAY — booking pending")
                : (paid ? "✅ Booking paid" : "📩 Pay clicked — order placed"),
            $"Order {order.OrderNumber} · {order.PaymentMethod} · {order.PaymentStatus}",
            ""
        };
        foreach (var item in experienceLines)
        {
            lines.Add($"• {item.ProductName}");
            lines.Add($"  Guests: {item.Quantity}");
        }

        AppendContact(lines, order.FullName, order.Phone, order.Email);
        if (!string.IsNullOrWhiteSpace(order.Address)
            && !string.Equals(order.Address, "Cusco experience", StringComparison.OrdinalIgnoreCase))
        {
            lines.Add($"Stay: {order.Address}");
        }

        if (!string.IsNullOrWhiteSpace(order.OrderNotes))
        {
            lines.Add($"Notes: {order.OrderNotes}");
        }

        return FireAndForgetAsync(string.Join('\n', lines), ct);
    }

    public Task NotifySlotAlarmAsync(
        string kind,
        Order order,
        OrderItem item,
        ExperienceSlot slot,
        CancellationToken ct = default)
    {
        var lima = BookingCalendarService.FormatSlot(slot.StartAt);
        var tourist = LooksLikeTourist(order.Phone, order.PaymentMethod);
        var title = kind switch
        {
            "day-before" => tourist ? "⏰ TOMORROW — tourist booking" : "⏰ TOMORROW — booking",
            "morning" => tourist ? "🔔 TODAY — tourist booking" : "🔔 TODAY — booking",
            _ => "Booking reminder"
        };

        var lines = new List<string>
        {
            title,
            lima,
            $"{item.ProductName} · {item.Quantity} guest(s)",
            $"Order {order.OrderNumber} · {order.PaymentStatus}",
            ""
        };
        AppendContact(lines, order.FullName, order.Phone, order.Email);
        return FireAndForgetAsync(string.Join('\n', lines), ct);
    }

    private async Task FireAndForgetAsync(string message, CancellationToken ct)
    {
        try
        {
            await _whatsApp.SendOpsAsync(message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ops booking notify failed.");
        }
    }

    private static void AppendCartLines(List<string> lines, IEnumerable<CartItem> items)
    {
        foreach (var item in items)
        {
            lines.Add($"• {item.ProductName}");
            lines.Add($"  Guests: {item.Quantity}");
        }
    }

    private static void AppendContact(List<string> lines, string? name, string? phone, string? email)
    {
        if (!string.IsNullOrWhiteSpace(name)) lines.Add($"Name: {name}");
        if (!string.IsNullOrWhiteSpace(phone)) lines.Add($"WhatsApp: {phone}");
        if (!string.IsNullOrWhiteSpace(email)) lines.Add($"Email: {email}");
    }

    public static bool IsExperience(string? productType) =>
        string.Equals(productType, "Experience", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Heuristic: foreign PayPal / non-Peru phone → treat as tourist (higher urgency).
    /// </summary>
    public static bool LooksLikeTourist(string? phone, string? paymentMethod)
    {
        if (PayPalService.IsPayPalMethod(paymentMethod) || PayPalService.IsUsd(paymentMethod))
        {
            return true;
        }

        var digits = new string((phone ?? "").Where(char.IsDigit).ToArray());
        if (digits.Length == 0)
        {
            return false;
        }

        if (digits.StartsWith("51", StringComparison.Ordinal) && digits.Length >= 11)
        {
            return false;
        }

        // Local mobile often entered as 9xxxxxxxx
        if (digits.Length == 9 && digits.StartsWith('9'))
        {
            return false;
        }

        return digits.Length >= 10;
    }
}
