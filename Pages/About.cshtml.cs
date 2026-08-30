using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;

namespace TheBestBean.Pages
{
    public class AboutModel : PageModel
    {
        private readonly ILogger<AboutModel> _logger;
        private readonly TheBestBeanContext _context;

        public AboutModel(ILogger<AboutModel> logger, TheBestBeanContext context)
        {
            _logger = logger;
            _context = context;
        }

        public Dictionary<string, string> PageContent { get; set; } = new Dictionary<string, string>();

        public async Task OnGetAsync()
        {
            PageContent = await _context.SiteContent.Where(c => c.Page == "About").ToDictionaryAsync(c => c.Key, c => c.Value);
        }
    }
}
