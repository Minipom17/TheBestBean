using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly TheBestBeanContext _context;

        public IndexModel(ILogger<IndexModel> logger, TheBestBeanContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IList<CoffeeBean> FeaturedProducts { get; set; } = default!;
        public IList<Experience> FeaturedExperiences { get; set; } = default!;
        public Dictionary<string, string> PageContent { get; set; } = new Dictionary<string, string>();

        public async Task OnGetAsync()
        {
            FeaturedProducts = await _context.CoffeeBean
                .Include(c => c.OriginCountry)
                .Take(5)
                .ToListAsync();

            FeaturedExperiences = await _context.Experiences
                .Where(e => e.Category != "Archived")
                .OrderBy(e =>
                    (!string.IsNullOrEmpty(e.Title) && e.Title.Contains("Coffee Laboratory")) ? 0 :
                    e.Tag == "FEATURED" ? 1 :
                    e.Tag == "1 HOUR" ? 2 : 3)
                .ThenBy(e => e.Category == "Urban Workshops" ? 0 : 1)
                .ThenBy(e => e.Id)
                .Take(5)
                .ToListAsync();
                
            PageContent = await _context.SiteContent.Where(c => c.Page == "Index").ToDictionaryAsync(c => c.Key, c => c.Value);
        }
    }
}
