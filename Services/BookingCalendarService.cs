using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Services
{
    public class BookingCalendarService
    {
        /// <summary>Default lead time for afternoon / evening sessions (Lima).</summary>
        public const int LeadHours = 4;
        /// <summary>Hard floor: never bookable this close unless BypassCutoff.</summary>
        public const int HardFloorMinutes = 30;
        /// <summary>Morning sessions (start before this Lima hour) close end of day before.</summary>
        public const int MorningHourExclusive = 12;
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

        /// <summary>
        /// Online bookable if capacity remains, not cancelled, and either:
        /// - admin set BypassCutoff, or
        /// - guests already booked (stay open to fill seats, until hard floor), or
        /// - morning (&lt;12 Lima): book on a previous calendar day, or
        /// - otherwise: more than LeadHours away.
        /// Hard floor (30 min) always applies unless BypassCutoff.
        /// </summary>
        public static bool IsOpenForBooking(ExperienceSlot slot, DateTime? nowUtc = null)
        {
            if (slot.IsCancelled) return false;
            if (slot.BookedCount >= slot.Capacity) return false;

            var now = nowUtc ?? DateTime.UtcNow;
            var start = DateTime.SpecifyKind(slot.StartAt, DateTimeKind.Utc);

            if (slot.BypassCutoff)
            {
                return start > now;
            }

            if (start <= now.AddMinutes(HardFloorMinutes))
            {
                return false;
            }

            // Someone already holds seats — keep the tour open to fill remaining capacity.
            if (slot.BookedCount > 0)
            {
                return true;
            }

            var limaNow = TimeZoneInfo.ConvertTimeFromUtc(now, LimaZone);
            var limaStart = TimeZoneInfo.ConvertTimeFromUtc(start, LimaZone);

            if (limaStart.Hour < MorningHourExclusive)
            {
                return limaNow.Date < limaStart.Date;
            }

            return start > now.AddHours(LeadHours);
        }

        public static string CutoffHint =>
            $"Mornings: book by the day before · Otherwise {LeadHours}-hour cutoff · Sundays closed";

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
            var now = DateTime.UtcNow;
            var slots = await _db.ExperienceSlots
                .Where(s => s.ExperienceId == experienceId
                    && !s.IsCancelled
                    && s.StartAt > now
                    && s.BookedCount < s.Capacity)
                .OrderBy(s => s.StartAt)
                .ToListAsync(ct);
            return slots.Where(s => IsOpenForBooking(s, now)).ToList();
        }

        public async Task<ExperienceSlot?> FindOpenSlotAsync(int slotId, int seats, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var slot = await _db.ExperienceSlots.FirstOrDefaultAsync(s =>
                s.Id == slotId
                && !s.IsCancelled
                && s.BookedCount + seats <= s.Capacity, ct);
            if (slot == null) return null;
            return IsOpenForBooking(slot, now) ? slot : null;
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
            var now = DateTime.UtcNow;
            var slots = await _db.ExperienceSlots
                .Where(s => ids.Contains(s.ExperienceId)
                    && !s.IsCancelled
                    && s.StartAt > now
                    && s.BookedCount < s.Capacity)
                .OrderBy(s => s.StartAt)
                .ToListAsync(ct);

            var open = slots.Where(s => IsOpenForBooking(s, now)).ToList();
            return ids.ToDictionary(
                id => id,
                id => open.FirstOrDefault(s => s.ExperienceId == id));
        }

        public async Task SetBypassCutoffAsync(int slotId, bool bypass, CancellationToken ct = default)
        {
            var slot = await _db.ExperienceSlots.FirstOrDefaultAsync(s => s.Id == slotId, ct);
            if (slot == null) return;
            slot.BypassCutoff = bypass;
            if (bypass)
            {
                slot.IsCancelled = false;
            }

            await _db.SaveChangesAsync(ct);
        }

        public async Task SetCancelledAsync(int slotId, bool cancelled, CancellationToken ct = default)
        {
            var slot = await _db.ExperienceSlots.FirstOrDefaultAsync(s => s.Id == slotId, ct);
            if (slot == null) return;
            slot.IsCancelled = cancelled;
            if (cancelled)
            {
                slot.BypassCutoff = false;
            }

            await _db.SaveChangesAsync(ct);
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
