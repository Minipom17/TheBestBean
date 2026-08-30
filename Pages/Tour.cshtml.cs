using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBestBean.Models;
using TheBestBean.Services;

namespace TheBestBean.Pages
{
    [IgnoreAntiforgeryToken]
    public class TourModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly TheBestBean.Data.TheBestBeanContext _context;

        public TourModel(CartService cartService, TheBestBean.Data.TheBestBeanContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public Experience? Tour { get; set; }
        public List<Experience> RelatedTours { get; set; } = new();
        public Dictionary<string, string> PageContent { get; set; } = new Dictionary<string, string>();
        public List<string> JsonLdBlocks { get; set; } = new();

        public IActionResult OnGet(string id)
        {
            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int tourId))
            {
                return NotFound();
            }

            Tour = _context.Experiences.FirstOrDefault(e => e.Id == tourId);

            if (Tour != null)
            {
                RelatedTours = _context.Experiences
                    .Where(t => t.Id != Tour.Id && t.Category == Tour.Category)
                    .Take(2)
                    .ToList();

                ApplyTourSeo(Tour);
            }

            PageContent = _context.SiteContent.Where(c => c.Page == "Tour").ToDictionary(c => c.Key, c => c.Value);

            return Page();
        }

        private void ApplyTourSeo(Experience tour)
        {
            var pageUrl = $"https://purplebean.coffee/Tour/{tour.Id}";
            ViewData["CanonicalUrl"] = pageUrl;
            ViewData["Title"] = tour.Title;
            ViewData["OgType"] = "article";
            ViewData["MetaImage"] = ToAbsolute(tour.ImageUrl);

            var meta = string.IsNullOrWhiteSpace(tour.LongDescription) ? tour.Description : tour.LongDescription;
            meta = string.Join(" ", meta.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            if (meta.Length > 155)
            {
                meta = meta[..152].TrimEnd() + "...";
            }
            ViewData["MetaDescription"] = meta;

            var images = new List<string> { ToAbsolute(tour.ImageUrl) };
            foreach (var galleryImage in tour.GalleryImages.Take(8))
            {
                var absolute = ToAbsolute(galleryImage);
                if (!images.Contains(absolute, StringComparer.OrdinalIgnoreCase))
                {
                    images.Add(absolute);
                }
            }

            var description = string.IsNullOrWhiteSpace(tour.LongDescription) ? tour.Description : tour.LongDescription;
            var price = tour.Price.ToString("0.00", CultureInfo.InvariantCulture);

            var listing = new Dictionary<string, object?>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = new[] { "TouristAttraction", "Service" },
                ["name"] = tour.Title,
                ["description"] = description,
                ["image"] = images,
                ["url"] = pageUrl,
                ["serviceType"] = "Coffee workshop",
                ["provider"] = new Dictionary<string, object?>
                {
                    ["@type"] = "CafeOrCoffeeShop",
                    ["name"] = "Purple Bean Coffee",
                    ["url"] = "https://purplebean.coffee/",
                    ["logo"] = "https://purplebean.coffee/images/brand-icon.png"
                },
                ["areaServed"] = new Dictionary<string, object?>
                {
                    ["@type"] = "City",
                    ["name"] = "Cusco"
                },
                ["offers"] = new Dictionary<string, object?>
                {
                    ["@type"] = "Offer",
                    ["url"] = pageUrl,
                    ["price"] = price,
                    ["priceCurrency"] = "USD",
                    ["availability"] = "https://schema.org/InStock"
                }
            };

            if (!string.IsNullOrWhiteSpace(tour.Duration))
            {
                listing["duration"] = tour.Duration;
            }

            var faq = new Dictionary<string, object?>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "FAQPage",
                ["mainEntity"] = new object[]
                {
                    Q($"How much does {tour.Title} cost?",
                        $"{tour.Title} is ${tour.Price.ToString("0", CultureInfo.InvariantCulture)} USD per participant at Purple Bean Coffee in Cusco, Peru."),
                    Q("How long is the coffee workshop in Cusco?",
                        string.IsNullOrWhiteSpace(tour.Duration)
                            ? "Workshop length varies by experience. Check the listing on purplebean.coffee for duration."
                            : $"{tour.Title} lasts {tour.Duration}."),
                    Q("Where is the Cusco coffee workshop?",
                        $"{tour.Title} is hosted by Purple Bean Coffee near San Pedro Market in Cusco, Peru. Book on purplebean.coffee or WhatsApp +51 993 779 381.")
                }
            };

            var crumbs = new Dictionary<string, object?>
            {
                ["@context"] = "https://schema.org",
                ["@type"] = "BreadcrumbList",
                ["itemListElement"] = new object[]
                {
                    new Dictionary<string, object?> { ["@type"] = "ListItem", ["position"] = 1, ["name"] = "Home", ["item"] = "https://purplebean.coffee/" },
                    new Dictionary<string, object?> { ["@type"] = "ListItem", ["position"] = 2, ["name"] = "Experiences", ["item"] = "https://purplebean.coffee/Experiences" },
                    new Dictionary<string, object?> { ["@type"] = "ListItem", ["position"] = 3, ["name"] = tour.Title, ["item"] = pageUrl }
                }
            };

            var jsonOpts = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
            JsonLdBlocks =
            [
                JsonSerializer.Serialize(listing, jsonOpts),
                JsonSerializer.Serialize(faq, jsonOpts),
                JsonSerializer.Serialize(crumbs, jsonOpts)
            ];
        }

        private static Dictionary<string, object?> Q(string question, string answer) => new()
        {
            ["@type"] = "Question",
            ["name"] = question,
            ["acceptedAnswer"] = new Dictionary<string, object?>
            {
                ["@type"] = "Answer",
                ["text"] = answer
            }
        };

        private static string ToAbsolute(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "https://purplebean.coffee/images/og-logo.png";
            }

            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }

            return "https://purplebean.coffee" + (path.StartsWith('/') ? path : "/" + path);
        }

        public IActionResult OnPostAddToCart(int tourId, string title, decimal price, string date, int participants = 1)
        {
            var cartItem = new CartItem
            {
                ProductId = tourId + 1000, // Offset to avoid ID collisions with shop products
                ProductName = $"{title} ({date})",
                ProductType = "Experience",
                Price = price,
                Quantity = participants,
                ImageUrl = "/Media/Cacao/Llama_Cacao_paper.webp",
                Description = $"Booking for {participants} person(s) on {date}"
            };

            _cartService.AddToCart(HttpContext.Session, cartItem);

            TempData["CartMessage"] = $"{participants}x {title} booking added to cart!";
            return RedirectToPage("/Cart");
        }


    }
}
