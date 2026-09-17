using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;
using TheBestBean.Services;

namespace TheBestBean.Pages
{
    [IgnoreAntiforgeryToken]
    public class ExperiencesModel : PageModel
    {
        private readonly TheBestBeanContext _context;
        private readonly BookingCalendarService _calendar;

        public ExperiencesModel(TheBestBeanContext context, BookingCalendarService calendar)
        {
            _context = context;
            _calendar = calendar;
        }

        public IList<Experience> ExperiencesList { get; set; } = default!;
        public Dictionary<string, string> PageContent { get; set; } = new Dictionary<string, string>();
        public Dictionary<int, string> NextSlotLabels { get; set; } = new();

        public async Task OnGetAsync()
        {
            ExperiencesList = await _context.Experiences
                .Where(e => e.Category != "Archived")
                .ToListAsync();
            PageContent = await _context.SiteContent.Where(c => c.Page == "Experiences").ToDictionaryAsync(c => c.Key, c => c.Value);

            await _calendar.EnsureUpcomingAsync();
            var next = await _calendar.NextSlotsAsync(ExperiencesList.Select(e => e.Id));
            NextSlotLabels = next.ToDictionary(
                kv => kv.Key,
                kv => kv.Value == null ? "Request a date" : "Next · " + BookingCalendarService.FormatSlot(kv.Value.StartAt));
        }
    }
}
