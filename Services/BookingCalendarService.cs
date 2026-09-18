using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Services
{
    public class BookingCalendarService
    {
        public const int LeadHours = 12;
        public const int HorizonDays = 56;

        private readonly TheBestBeanContext _db;

        public BookingCalendarService(TheBestBeanContext db)
        {
            _db = db;
        }

        public static TimeZoneInfo LimaZone
        {
            get
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
                }
                catch (TimeZoneNotFoundException)
                {
                    return TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
                }
            }
        }

        public static DateTime NowUtc() => DateTime.UtcNow;

        public static string FormatSlot(DateTime startUtc)
        {
            var utc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
            var lima = TimeZoneInfo.ConvertTimeFromUtc(utc, LimaZone);
            return lima.ToString("ddd d MMM · HH:mm", System.Globalization.CultureInfo.GetCultureInfo("en-GB"));
        }

        public static string FormatDay(DateTime startUtc)
        {
            var utc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
            var lima = TimeZoneInfo.ConvertTimeFromUtc(utc, LimaZone);
            return lima.ToString("yyyy-MM-dd");
        }

        public static DateTime ToLima(DateTime startUtc)
        {
            var utc = DateTime.SpecifyKind(startUtc, DateTimeKind.Utc);
            return TimeZoneInfo.ConvertTimeFromUtc(utc, LimaZone);
        }

        public async Task EnsureUpcomingAsync(int? experienceId = null, CancellationToken ct = default)
        {
            var query = _db.Experiences.AsQueryable().Where(e => e.Category != "Archived");
            if (experienceId.HasValue)
            {
                query = query.Where(e => e.Id == experienceId.Value);
            }

            var experiences = await query.ToListAsync(ct);
            var now = DateTime.UtcNow;
            var horizon = now.AddDays(HorizonDays);

            foreach (var exp in experiences)
            {
                var existing = await _db.ExperienceSlots
                    .AsNoTracking()
                    .Where(s => s.ExperienceId == exp.Id)
                    .Select(s => s.StartAt)
                    .ToListAsync(ct);
                var have = existing.SelectMany(ExistingKeys).ToHashSet();

                foreach (var start in ProposeStarts(exp, now, horizon))
                {
                    if (have.Contains(UtcMinuteKey(start))) continue;
                    have.Add(UtcMinuteKey(start));
                    _db.ExperienceSlots.Add(new ExperienceSlot
                    {
                        ExperienceId = exp.Id,
                        StartAt = DateTime.SpecifyKind(start, DateTimeKind.Utc),
                        Capacity = DefaultCapacity(exp),
                        BookedCount = 0
                    });
                }
            }

            if (_db.ChangeTracker.HasChanges())
            {
                try
                {
                    await _db.SaveChangesAsync(ct);
                }
                catch (Exception)
                {
                    foreach (var entry in _db.ChangeTracker.Entries<ExperienceSlot>().Where(e => e.State == EntityState.Added))
                    {
                        entry.State = EntityState.Detached;
                    }
                }
            }
        }

        public async Task<List<ExperienceSlot>> OpenSlotsAsync(int experienceId, CancellationToken ct = default)
        {
            var cutoff = DateTime.UtcNow.AddHours(LeadHours);
            return await _db.ExperienceSlots
                .Where(s => s.ExperienceId == experienceId
                    && !s.IsCancelled
                    && s.StartAt > cutoff
                    && s.BookedCount < s.Capacity)
                .OrderBy(s => s.StartAt)
                .ToListAsync(ct);
        }

        public async Task<ExperienceSlot?> FindOpenSlotAsync(int slotId, int seats, CancellationToken ct = default)
        {
            var cutoff = DateTime.UtcNow.AddHours(LeadHours);
            return await _db.ExperienceSlots.FirstOrDefaultAsync(s =>
                s.Id == slotId
                && !s.IsCancelled
                && s.StartAt > cutoff
                && s.BookedCount + seats <= s.Capacity, ct);
        }

        public async Task ReserveAsync(IEnumerable<(int SlotId, int Seats)> holds, CancellationToken ct = default)
        {
            foreach (var group in holds.GroupBy(h => h.SlotId))
            {
                var slot = await _db.ExperienceSlots.FirstOrDefaultAsync(s => s.Id == group.Key, ct);
                if (slot == null) continue;
                slot.BookedCount += group.Sum(h => h.Seats);
            }

            await _db.SaveChangesAsync(ct);
        }

        public async Task<Dictionary<int, ExperienceSlot?>> NextSlotsAsync(IEnumerable<int> experienceIds, CancellationToken ct = default)
        {
            var ids = experienceIds.Distinct().ToList();
            var cutoff = DateTime.UtcNow.AddHours(LeadHours);
            var slots = await _db.ExperienceSlots
                .Where(s => ids.Contains(s.ExperienceId)
                    && !s.IsCancelled
                    && s.StartAt > cutoff
                    && s.BookedCount < s.Capacity)
                .OrderBy(s => s.StartAt)
                .ToListAsync(ct);

            return ids.ToDictionary(
                id => id,
                id => slots.FirstOrDefault(s => s.ExperienceId == id));
        }

        private static string UtcMinuteKey(DateTime value)
        {
            var utc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            return new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, 0, DateTimeKind.Utc).ToString("yyyy-MM-dd HH:mm");
        }

        private static IEnumerable<string> ExistingKeys(DateTime stored)
        {
            yield return UtcMinuteKey(stored);
            var limaWall = DateTime.SpecifyKind(
                new DateTime(stored.Year, stored.Month, stored.Day, stored.Hour, stored.Minute, 0),
                DateTimeKind.Unspecified);
            yield return UtcMinuteKey(TimeZoneInfo.ConvertTimeToUtc(limaWall, LimaZone));
        }

        private static int DefaultCapacity(Experience exp)
        {
            var d = (exp.Duration ?? "").ToLowerInvariant();
            if (d.Contains("day")) return 6;
            if (d.Contains("2.5") || d.Contains("2 hour") || d.Contains("3 hour")) return 6;
            return 8;
        }

        private static IEnumerable<DateTime> ProposeStarts(Experience exp, DateTime fromUtc, DateTime toUtc)
        {
            var lima = LimaZone;
            var fromLima = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(fromUtc, DateTimeKind.Utc), lima).Date;
            var toLima = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(toUtc, DateTimeKind.Utc), lima).Date;
            var times = SessionTimes(exp);
            var days = SessionDays(exp);

            for (var day = fromLima; day <= toLima; day = day.AddDays(1))
            {
                if (!days.Contains(day.DayOfWeek)) continue;
                foreach (var time in times)
                {
                    var local = DateTime.SpecifyKind(day.Add(time), DateTimeKind.Unspecified);
                    yield return TimeZoneInfo.ConvertTimeToUtc(local, lima);
                }
            }
        }

        private static HashSet<DayOfWeek> SessionDays(Experience exp)
        {
            var d = (exp.Duration ?? "").ToLowerInvariant();
            var category = (exp.Category ?? "").ToLowerInvariant();
            if (d.Contains("2 day") || d.Contains("2 days") || d.Contains("multi"))
            {
                return [DayOfWeek.Saturday];
            }

            if (d.Contains("1 day") || d.Contains("day") && category.Contains("expedition"))
            {
                return [DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday];
            }

            return
            [
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday
            ];
        }

        private static TimeSpan[] SessionTimes(Experience exp)
        {
            var d = (exp.Duration ?? "").ToLowerInvariant();
            if (d.Contains("2 day") || d.Contains("2 days") || d.Contains("1 day") || (d.Contains("day") && !d.Contains("hour")))
            {
                return [new TimeSpan(7, 0, 0)];
            }

            if (d.Contains("2.5") || d.Contains("2 hour") || d.Contains("3 hour"))
            {
                return [new TimeSpan(9, 0, 0), new TimeSpan(14, 0, 0)];
            }

            return
            [
                new TimeSpan(10, 0, 0),
                new TimeSpan(12, 0, 0),
                new TimeSpan(14, 0, 0),
                new TimeSpan(16, 0, 0)
            ];
        }
    }
}
