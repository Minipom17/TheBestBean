using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;

namespace TheBestBean.Services;

/// <summary>
/// Day-before and morning-of WhatsApp alarms for booked experience slots.
/// </summary>
public class BookingReminderHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopes;
    private readonly ILogger<BookingReminderHostedService> _logger;

    public BookingReminderHostedService(IServiceScopeFactory scopes, ILogger<BookingReminderHostedService> logger)
    {
        _scopes = scopes;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Stagger first tick so startup migrations finish.
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(45), stoppingToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await TickAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Booking reminder tick failed.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(12), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private async Task TickAsync(CancellationToken ct)
    {
        using var scope = _scopes.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TheBestBeanContext>();
        var ops = scope.ServiceProvider.GetRequiredService<BookingOpsNotifyService>();
        if (!ops.IsConfigured)
        {
            return;
        }

        var limaNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BookingCalendarService.LimaZone);
        var tomorrow = limaNow.Date.AddDays(1);
        var today = limaNow.Date;

        // Day-before window: after 17:00 Lima, for sessions tomorrow.
        if (limaNow.Hour >= 17)
        {
            await SendWindowAsync(db, ops, "day-before", tomorrow, item => !item.DayBeforeAlertSent, (item) =>
            {
                item.DayBeforeAlertSent = true;
            }, ct);
        }

        // Morning-of window: after 06:30 Lima, for sessions today.
        if (limaNow.Hour > 6 || (limaNow.Hour == 6 && limaNow.Minute >= 30))
        {
            await SendWindowAsync(db, ops, "morning", today, item => !item.MorningOfAlertSent, (item) =>
            {
                item.MorningOfAlertSent = true;
            }, ct);
        }
    }

    private async Task SendWindowAsync(
        TheBestBeanContext db,
        BookingOpsNotifyService ops,
        string kind,
        DateTime limaDay,
        Func<Models.OrderItem, bool> needsSend,
        Action<Models.OrderItem> markSent,
        CancellationToken ct)
    {
        var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(
            DateTime.SpecifyKind(limaDay, DateTimeKind.Unspecified),
            BookingCalendarService.LimaZone);
        var dayEndUtc = dayStartUtc.AddDays(1);

        var rows = await (
            from item in db.OrderItems.Include(i => i.Order)
            join slot in db.ExperienceSlots on item.SlotId equals slot.Id
            where item.SlotId > 0
                && item.ProductType == "Experience"
                && slot.StartAt >= dayStartUtc
                && slot.StartAt < dayEndUtc
                && item.Order != null
                && item.Order.PaymentStatus != "Failed"
                && item.Order.PaymentStatus != "Cancelled"
            select new { Item = item, Slot = slot, Order = item.Order! }
        ).ToListAsync(ct);

        var dirty = false;
        foreach (var row in rows)
        {
            if (!needsSend(row.Item)) continue;
            await ops.NotifySlotAlarmAsync(kind, row.Order, row.Item, row.Slot, ct);
            markSent(row.Item);
            dirty = true;
            _logger.LogInformation(
                "Sent {Kind} booking alarm for order {Order} slot {Slot}.",
                kind,
                row.Order.OrderNumber,
                row.Slot.Id);
        }

        if (dirty)
        {
            await db.SaveChangesAsync(ct);
        }
    }
}
