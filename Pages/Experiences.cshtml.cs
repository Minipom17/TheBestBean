using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages
{
    [IgnoreAntiforgeryToken]
    public class ExperiencesModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public ExperiencesModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public IList<Experience> ExperiencesList { get; set; } = default!;
        public Dictionary<string, string> PageContent { get; set; } = new Dictionary<string, string>();

        public async Task OnGetAsync()
        {
            ExperiencesList = await _context.Experiences.ToListAsync();
            PageContent = await _context.SiteContent.Where(c => c.Page == "Experiences").ToDictionaryAsync(c => c.Key, c => c.Value);
        }
    }
}
