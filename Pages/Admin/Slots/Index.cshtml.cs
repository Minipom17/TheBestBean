using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;
using TheBestBean.Services;

namespace TheBestBean.Pages.Admin.Slots
{
    public class IndexModel : PageModel
    {
        private readonly TheBestBeanContext _db;
        private readonly BookingCalendarService _calendar;

        public IndexModel(TheBestBeanContext db, BookingCalendarService calendar)
        {
            _db = db;
            _calendar = calendar;
        }

        public IList<Experience> Experiences { get; set; } = new List<Experience>();
        public IList<SlotRow> Slots { get; set; } = new List<SlotRow>();
        public SelectList ExperienceOptions { get; set; } = default!;

        [BindProperty(SupportsGet = true)]
        public int? ExperienceId { get; set; }

        public string StatusMessage { get; set; } = string.Empty;

        public class SlotRow
        {
            public ExperienceSlot Slot { get; set; } = default!;
            public string ExperienceTitle { get; set; } = "";
            public bool OnlineOpen { get; set; }
            public string LimaLabel { get; set; } = "";
        }

        public async Task OnGetAsync()
        {
            await LoadAsync();
        }

        public async Task<IActionResult> OnPostOpenAsync(int slotId)
        {
            await _calendar.SetBypassCutoffAsync(slotId, true);
            StatusMessage = "Slot opened for online booking (cutoff bypassed).";
            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostClearBypassAsync(int slotId)
        {
            await _calendar.SetBypassCutoffAsync(slotId, false);
            StatusMessage = "Cutoff override cleared.";
            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int slotId)
        {
            await _calendar.SetCancelledAsync(slotId, true);
            StatusMessage = "Slot cancelled.";
            await LoadAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostRestoreAsync(int slotId)
        {
            await _calendar.SetCancelledAsync(slotId, false);
            StatusMessage = "Slot restored (still respects cutoff unless you Open it).";
            await LoadAsync();
            return Page();
        }

        private async Task LoadAsync()
        {
            Experiences = await _db.Experiences
                .Where(e => e.Category != "Archived")
                .OrderBy(e => e.Title)
                .ToListAsync();
            ExperienceOptions = new SelectList(Experiences, nameof(Experience.Id), nameof(Experience.Title), ExperienceId);

            if (ExperienceId.HasValue)
            {
                await _calendar.EnsureUpcomingAsync(ExperienceId.Value);
            }

            var now = DateTime.UtcNow;
            var query = _db.ExperienceSlots
                .Include(s => s.Experience)
                .Where(s => s.StartAt > now.AddHours(-6))
                .AsQueryable();

            if (ExperienceId.HasValue)
            {
                query = query.Where(s => s.ExperienceId == ExperienceId.Value);
            }

            var slots = await query.OrderBy(s => s.StartAt).Take(80).ToListAsync();
            Slots = slots.Select(s => new SlotRow
            {
                Slot = s,
                ExperienceTitle = s.Experience?.Title ?? $"#{s.ExperienceId}",
                OnlineOpen = BookingCalendarService.IsOpenForBooking(s, now),
                LimaLabel = BookingCalendarService.FormatSlot(s.StartAt)
            }).ToList();
        }
    }
}
