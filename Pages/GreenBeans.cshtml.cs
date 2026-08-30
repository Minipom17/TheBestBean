using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages
{
    [IgnoreAntiforgeryToken]
    public class GreenBeansModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public GreenBeansModel(TheBestBeanContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Type { get; set; }

        public IList<CoffeeBean> Products { get; set; } = default!;

        public Dictionary<string, string> PageContent { get; set; } = new Dictionary<string, string>();
        public List<FlavorZone> FlavorZones { get; set; } = new List<FlavorZone>();

        public async Task OnGetAsync()
        {
            Products = await _context.CoffeeBean
                .Include(b => b.CoffeeFarm)
                .Include(b => b.CoffeeRegion)
                .Include(b => b.OriginCountry)
                .ToListAsync();

            PageContent = await _context.SiteContent.Where(c => c.Page == "GreenBeans").ToDictionaryAsync(c => c.Key, c => c.Value);
            FlavorZones = await _context.FlavorZones.ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (User.Identity?.IsAuthenticated != true) return Unauthorized();

            var coffee = await _context.CoffeeBean.FindAsync(id);
            if (coffee != null)
            {
                _context.CoffeeBean.Remove(coffee);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
