using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using TheBestBean.Data;
using TheBestBean.Models;
using TheBestBean.Services;

namespace TheBestBean.Pages
{
    [IgnoreAntiforgeryToken]
    public class GreenBeansModel : PageModel
    {
        private readonly TheBestBeanContext _context;
        private readonly CoffeeFreshnessService _freshness;

        public GreenBeansModel(TheBestBeanContext context, CoffeeFreshnessService freshness)
        {
            _context = context;
            _freshness = freshness;
        }

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Type { get; set; }

        public IList<CoffeeBean> Products { get; set; } = default!;

        public Dictionary<string, string> PageContent { get; set; } = new Dictionary<string, string>();
        public List<FlavorZone> FlavorZones { get; set; } = new List<FlavorZone>();
        public CoffeeLabBoard LabBoard { get; set; } = new();
        public Dictionary<int, CoffeeLotStatus> LotsById { get; set; } = new();
        public List<object> ShopLotsJson { get; set; } = new();

        public async Task OnGetAsync()
        {
            Products = await _context.CoffeeBean
                .Include(b => b.CoffeeFarm)
                .Include(b => b.CoffeeRegion)
                .Include(b => b.OriginCountry)
                .ToListAsync();

            PageContent = await _context.SiteContent.Where(c => c.Page == "GreenBeans").ToDictionaryAsync(c => c.Key, c => c.Value);
            FlavorZones = await _context.FlavorZones.ToListAsync();
            var live = await _freshness.LiveBoardAsync();
            LotsById = live.Lots.ToDictionary(l => l.CoffeeBeanId);
            LabBoard = new CoffeeLabBoard
            {
                Lots = live.Lots
                    .Where(l => l.HasGreen || l.HasRoast)
                    .Take(10)
                    .ToList(),
                Compact = true,
                ShowTimeline = false,
                ShowGrind = false,
                ShowLede = false,
                Kicker = live.Kicker,
                Title = live.Title,
                Highlight = live.Highlight
            };
            ViewData["CoffeeLots"] = LotsById;

            ShopLotsJson = Products.Select(p =>
            {
                LotsById.TryGetValue(p.Id, out var lot);
                return (object)new
                {
                    id = p.Id,
                    name = CoffeeDisplayName.ForLab(p.Name),
                    flavor = p.FlavorProfile ?? "",
                    process = p.ProcessingMethod ?? "",
                    altitude = p.Altitude ?? "",
                    variety = p.Variety ?? "",
                    origin = p.OriginCountry?.Name ?? "Peru",
                    region = p.CoffeeRegion?.Name ?? "",
                    sca = p.ScaScore,
                    greenKg = lot?.GreenKg ?? 0m,
                    hasGreen = lot?.HasGreen ?? false
                };
            }).ToList();
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
