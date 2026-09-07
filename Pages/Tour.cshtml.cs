using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Localization;
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
        public bool IsCoffeeLab { get; set; }
        public bool IsSpanish { get; set; }
        public IReadOnlyList<LabRecipeCard> LabRecipes { get; set; } = Array.Empty<LabRecipeCard>();

        public IActionResult OnGet(string id)
        {
            IsSpanish = HttpContext.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.TwoLetterISOLanguageName == "es";

            if (string.IsNullOrEmpty(id) || !int.TryParse(id, out int tourId))
            {
                return NotFound();
            }

            Tour = _context.Experiences.FirstOrDefault(e => e.Id == tourId);

            if (Tour != null)
            {
                RelatedTours = _context.Experiences
                    .Where(t => t.Id != Tour.Id && t.Category == Tour.Category)
                    .OrderBy(t => t.SortOrder)
                    .ThenBy(t => t.Id)
                    .Take(2)
                    .ToList();

                IsCoffeeLab = Tour.IsCuscoCoffeeLab;
                if (IsCoffeeLab)
                {
                    LabRecipes = CoffeeLabRecipes;
                }

                ApplyTourSeo(Tour);
                Ga4Ecommerce.SetPageEvent(ViewData, "view_item", Ga4Ecommerce.Payload(new[]
                {
                    new CartItem
                    {
                        ProductId = Tour.Id,
                        ProductName = Tour.Title,
                        ProductType = "Experience",
                        Price = Tour.Price,
                        Quantity = 1
                    }
                }));
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
                        tour.IsSanBlasPourOver
                            ? $"{tour.Title} meets at San Pedro Market, then walks to a hotel on Plaza San Blas in Cusco, Peru (exact hotel pin TBD). Book on purplebean.coffee or WhatsApp +51 993 779 381."
                            : $"{tour.Title} is hosted by Purple Bean Coffee near San Pedro Market in Cusco, Peru. Book on purplebean.coffee or WhatsApp +51 993 779 381.")
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

        public static readonly IReadOnlyList<LabRecipeCard> CoffeeLabRecipes =
        [
            new LabRecipeCard
            {
                Code = "01",
                Method = "V60",
                Kind = "Pour-over",
                Dose = "15 g",
                Yield = "250 g",
                Time = "2:45",
                Grind = "Medium-fine",
                Temp = "96 °C",
                Ratio = "1 : 16.5",
                Steps =
                [
                    "Rinse the paper filter and warm the carafe.",
                    "Bloom with 45 g water for 45 seconds.",
                    "Pour in slow concentric circles to 250 g.",
                    "Let the bed drain. Total time about 2:45."
                ],
                Tip = "Keep the pour gentle — the cone is unforgiving and shows floral, citrus cups from Cusco lots."
            },
            new LabRecipeCard
            {
                Code = "02",
                Method = "Espresso",
                Kind = "Pressure",
                Dose = "18 g",
                Yield = "36 g",
                Time = "28–32 s",
                Grind = "Fine / espresso",
                Temp = "93 °C",
                Ratio = "1 : 2",
                Steps =
                [
                    "Dose 18 g, distribute, and tamp level.",
                    "Start the shot. Aim for 36 g in 28–32 seconds.",
                    "If it gushes, grind finer. If it chokes, grind coarser.",
                    "Taste: sweetness first, then acidity, then finish."
                ],
                Tip = "Change one variable at a time. Grind is the first lever in the lab."
            },
            new LabRecipeCard
            {
                Code = "03",
                Method = "AeroPress",
                Kind = "Immersion",
                Dose = "18 g",
                Yield = "200 g",
                Time = "2:00",
                Grind = "Medium-fine",
                Temp = "92 °C",
                Ratio = "1 : 11",
                Steps =
                [
                    "Inverted: add 18 g coffee, then 200 g water.",
                    "Stir hard for 10 seconds. Steep to 1:30.",
                    "Cap, flip onto the mug, press steadily for 30 seconds.",
                    "Stop when you hear the hiss. Don’t squeeze the puck dry."
                ],
                Tip = "Full body, low bitterness — a travel brew guests can repeat at home."
            },
            new LabRecipeCard
            {
                Code = "04",
                Method = "Cupping",
                Kind = "Sensory",
                Dose = "8.25 g",
                Yield = "150 g",
                Time = "4:00 +",
                Grind = "Coarse / cupping",
                Temp = "93 °C",
                Ratio = "1 : 18",
                Steps =
                [
                    "Grind 8.25 g into the bowl. Smell the dry fragrance.",
                    "Pour 150 g water. Steep 4 minutes.",
                    "Break the crust, smell the wet aroma, skim the foam.",
                    "Taste with a cupping spoon from hot to cool."
                ],
                Tip = "Score sweetness, acidity, body, and aftertaste. Cooler cups tell the truth."
            }
        ];

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
            Ga4Ecommerce.QueueAddToCart(TempData, cartItem);

            TempData["CartMessage"] = $"{participants}x {title} booking added to cart!";
            return RedirectToPage("/Cart");
        }
    }

    public sealed class LabRecipeCard
    {
        public required string Code { get; init; }
        public required string Method { get; init; }
        public required string Kind { get; init; }
        public required string Dose { get; init; }
        public required string Yield { get; init; }
        public required string Time { get; init; }
        public required string Grind { get; init; }
        public required string Temp { get; init; }
        public required string Ratio { get; init; }
        public required string[] Steps { get; init; }
        public required string Tip { get; init; }
    }
}
