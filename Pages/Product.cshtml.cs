using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TheBestBean.Models;
using TheBestBean.Services;

namespace TheBestBean.Pages
{
    [IgnoreAntiforgeryToken]
    public class ProductModel : PageModel
    {
        private readonly CartService _cartService;
        private readonly TheBestBean.Data.TheBestBeanContext _context;

        public ProductModel(CartService cartService, TheBestBean.Data.TheBestBeanContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public Product? Product { get; set; }
        public List<Product> RelatedProducts { get; set; } = new();
        public Dictionary<string, string> PageContent { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            int.TryParse(id, out int productId);
            var bean = await _context.CoffeeBean
                .Include(b => b.OriginCountry)
                .Include(b => b.CoffeeRegion)
                .FirstOrDefaultAsync(b => b.Id == productId || b.Name == id || b.Name.Replace(" ", "-") == id);

            PageContent = await _context.SiteContent
                .Where(c => c.Key.StartsWith($"Product_{productId}_") || c.Page == "Product")
                .ToDictionaryAsync(c => c.Key, c => c.Value);

            if (bean != null)
            {
                Product = new Product
                {
                    Id = bean.Id,
                    Name = bean.Name,
                    Origin = bean.CoffeeRegion?.Name ?? bean.OriginCountry?.Name ?? "Peru",
                    Type = "Bean",
                    Price = bean.BasePriceUSD,
                    BasePriceUSD = bean.BasePriceUSD,
                    BasePricePEN = bean.BasePricePEN,
                    Unit = "100g",
                    Rating = bean.Rating,
                    ImageUrl = bean.ImageUrl,
                    Description = bean.FlavorProfile,
                    FlavorProfile = bean.FlavorProfile,
                    ProcessingMethod = bean.ProcessingMethod,
                    ProducerImageUrl = bean.ProducerImageUrl,
                    ProducerDescription = bean.ProducerDescription,
                    ProcessingDescription = bean.ProcessingDescription,
                    MapImageUrl = bean.MapImageUrl,
                    OriginDescription = bean.OriginDescription,
                    Variety = bean.Variety,
                    Altitude = bean.Altitude,
                    Producer = bean.Producer ?? string.Empty,
                    ScaScore = bean.ScaScore,
                    GalleryImages = AngelCatarataGallery(bean)
                };
            }
            else 
            {
                // Fallback to mock products for accessories or cacao if needed
                var products = GetMockProducts();
                Product = products.FirstOrDefault(p => p.Id == productId || p.Name == id || p.Name.Replace(" ", "-") == id);
            }

            if (Product == null)
            {
                return NotFound();
            }

            ViewData["Title"] = Product.Name;
            ViewData["MetaDescription"] = !string.IsNullOrEmpty(Product.FlavorProfile) ? $"Experience {Product.Name}. {Product.FlavorProfile}" : Product.Description;
            ViewData["MetaImage"] = $"https://purplebean.coffee{Product.ImageUrl}";
            ViewData["OgType"] = "product";
            Ga4Ecommerce.SetPageEvent(ViewData, "view_item", Ga4Ecommerce.Payload(new[]
            {
                new CartItem
                {
                    ProductId = Product.Id,
                    ProductName = Product.Name,
                    ProductType = Product.Type,
                    Price = Product.BasePriceUSD > 0 ? Product.BasePriceUSD : Product.Price,
                    Quantity = 1
                }
            }));

            return Page();
        }



        public IActionResult OnPostAddToCart(int productId, string productName, string productType, decimal price, string imageUrl, string description, int quantity = 1, string? purchaseType = null, string? subscriptionFrequency = null, string? weight = "100g")
        {
            var bean = _context.CoffeeBean.Find(productId);
            if (bean != null)
            {
                price = LocalPricing.CoffeeBagUsd(bean.ScaScore, weight);
                if (!string.IsNullOrWhiteSpace(weight))
                {
                    productName = $"{productName} ({weight})";
                }
            }

            var finalPrice = price;
            var finalName = productName;
            var finalDesc = description;

            if (purchaseType == "Subscribe")
            {
                finalPrice = Math.Round(price * 0.9m, 2); // 10% discount for subscription
                var freqText = string.IsNullOrEmpty(subscriptionFrequency) ? "Every 2 Weeks" : subscriptionFrequency;
                finalName = $"{productName} (Subscription - {freqText})";
                finalDesc = $"{description} [Subscription: {freqText}]";
            }

            var cartItem = new TheBestBean.Models.CartItem
            {
                ProductId = productId,
                ProductName = finalName,
                ProductType = productType,
                Price = finalPrice,
                Quantity = quantity,
                ImageUrl = imageUrl,
                Description = finalDesc
            };

            _cartService.AddToCart(HttpContext.Session, cartItem);
            Ga4Ecommerce.QueueAddToCart(TempData, cartItem);

            TempData["CartMessage"] = $"{quantity}x {finalName} added to cart!";
            return RedirectToPage("/Product", new { id = productId });
        }

        private static List<string> AngelCatarataGallery(CoffeeBean bean)
        {
            var isAngel = (bean.Name ?? string.Empty).Contains("Catarata", StringComparison.OrdinalIgnoreCase)
                || (bean.Producer ?? string.Empty).Contains("Manosalva", StringComparison.OrdinalIgnoreCase)
                || (bean.Producer ?? string.Empty).Contains("Ángel", StringComparison.OrdinalIgnoreCase);
            if (!isAngel)
            {
                return new List<string>();
            }

            return
            [
                "/Media/producers/angel-cajamarca/farm-view.jpg?v=4",
                "/Media/producers/angel-cajamarca/cherries-cluster.jpg?v=4",
                "/Media/producers/angel-cajamarca/harvest-sack.jpg?v=4",
                "/Media/producers/angel-cajamarca/wooden-bin.jpg?v=4",
                "/Media/producers/angel-cajamarca/depulper.jpg?v=4",
                "/Media/producers/angel-cajamarca/cherries-branch.jpg?v=4"
            ];
        }

        private List<Product> GetMockProducts()
        {
            return new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Ethiopian Heirloom",
                    Origin = "Yirgacheffe, Ethiopia",
                    Type = "Bean",
                    Price = 24.99m,
                    Unit = "12oz",
                    Rating = 4.9m,
                    ImageUrl = "/Media/Shop/Green_bean.svg",
                    Description = "Jasmine, Blueberry, Citrus notes with bright acidity",
                    FlavorProfile = "Jasmine, Blueberry, Citrus",
                    ProcessingMethod = "Washed"
                },
                new Product
                {
                    Id = 2,
                    Name = "Colombian Supremo",
                    Origin = "Huila, Colombia",
                    Type = "Bean",
                    Price = 19.99m,
                    Unit = "12oz",
                    Rating = 4.5m,
                    ImageUrl = "/Media/Green_bean.svg",
                    Description = "Bright acidity, Chocolate notes, Clean finish",
                    FlavorProfile = "Chocolate, Clean finish",
                    ProcessingMethod = "Washed"
                },
                new Product
                {
                    Id = 3,
                    Name = "Peruvian Typica Pacamara",
                    Origin = "Cusco, Peru",
                    Type = "Bean",
                    Price = 22.99m,
                    Unit = "12oz",
                    Rating = 4.7m,
                    ImageUrl = "/Media/Green_bean.svg",
                    Description = "Caramel, Walnut, Brown Sugar sweetness",
                    FlavorProfile = "Caramel, Walnut, Brown Sugar",
                    ProcessingMethod = "Natural"
                },
                new Product
                {
                    Id = 4,
                    Name = "SL09",
                    Origin = "La Convención, Cusco, Peru",
                    Type = "Bean",
                    Price = 32.89m,
                    Unit = "12oz",
                    Rating = 5.0m,
                    ImageUrl = "/Media/Shop/Green_bean.svg",
                    Description = "Our flagship coffee from La Convención. Chocolate and caramel notes with clean, bright acidity. Currently in stock and available for both local sale and international export.",
                    FlavorProfile = "Chocolate, Caramel, Bright",
                    ProcessingMethod = "Washed"
                },
                new Product
                {
                    Id = 5,
                    Name = "SL09 Light Roast",
                    Origin = "La Convención, Cusco, Peru",
                    Type = "Bean",
                    Price = 32.89m,
                    Unit = "12oz",
                    Rating = 4.9m,
                    ImageUrl = "/Media/Shop/Green_bean.svg",
                    Description = "Light roasted SL09 beans highlighting bright acidity and floral notes. Perfect for pour-over and filter brewing methods.",
                    FlavorProfile = "Bright, Floral, Citrus",
                    ProcessingMethod = "Washed"
                },
                new Product
                {
                    Id = 6,
                    Name = "SL09 Medium Roast",
                    Origin = "La Convención, Cusco, Peru",
                    Type = "Bean",
                    Price = 32.89m,
                    Unit = "12oz",
                    Rating = 5.0m,
                    ImageUrl = "/Media/Shop/Green_bean.svg",
                    Description = "Medium roasted SL09 beans with balanced chocolate and caramel notes. Versatile for all brewing methods.",
                    FlavorProfile = "Chocolate, Caramel, Balanced",
                    ProcessingMethod = "Washed"
                },
                new Product
                {
                    Id = 7,
                    Name = "SL09 Dark Roast",
                    Origin = "La Convención, Cusco, Peru",
                    Type = "Bean",
                    Price = 32.89m,
                    Unit = "12oz",
                    Rating = 4.8m,
                    ImageUrl = "/Media/Shop/Green_bean.svg",
                    Description = "Dark roasted SL09 beans with rich, bold flavors. Perfect for espresso and French press.",
                    FlavorProfile = "Bold, Rich, Chocolate",
                    ProcessingMethod = "Washed"
                },
                new Product
                {
                    Id = 8,
                    Name = "La Convención",
                    Origin = "La Convención, Cusco, Peru",
                    Type = "Bean",
                    Price = 31.05m,
                    Unit = "12oz",
                    Rating = 4.7m,
                    ImageUrl = "/Media/Shop/Green_bean.svg",
                    Description = "Medium roasted coffee from La Convención region. Smooth body with notes of caramel and nuts.",
                    FlavorProfile = "Caramel, Nuts, Smooth",
                    ProcessingMethod = "Washed"
                },
                // Cacao Products
                new Product
                {
                    Id = 9,
                    Name = "Raw Cacao Beans",
                    Origin = "Cusco, Peru",
                    Type = "Cacao",
                    Price = 22.37m,
                    Unit = "1kg",
                    Rating = 4.8m,
                    ImageUrl = "/Media/Cacao/cacao_bean.webp",
                    Description = "Premium raw cacao beans from Cusco region. Perfect for making your own chocolate or cacao products.",
                    FlavorProfile = "Rich, Fruity, Earthy",
                    ProcessingMethod = "Fermented & Dried"
                },
                new Product
                {
                    Id = 10,
                    Name = "Cacao Nibs",
                    Origin = "Cusco, Peru",
                    Type = "Cacao",
                    Price = 11.84m,
                    Unit = "250g",
                    Rating = 4.9m,
                    ImageUrl = "/Media/Cacao/cacao_nibs.webp",
                    Description = "Organic cacao nibs, perfect for adding to smoothies, yogurt, or baking. Rich in antioxidants.",
                    FlavorProfile = "Nutty, Chocolate, Bitter",
                    ProcessingMethod = "Fermented & Dried"
                },
                new Product
                {
                    Id = 11,
                    Name = "Cacao Powder",
                    Origin = "Cusco, Peru",
                    Type = "Cacao",
                    Price = 17.11m,
                    Unit = "500g",
                    Rating = 4.7m,
                    ImageUrl = "/Media/Cacao/cacao_powder.webp",
                    Description = "Organic cacao powder for baking and beverages. Rich, dark, and full of flavor.",
                    FlavorProfile = "Rich, Bitter, Chocolate",
                    ProcessingMethod = "Fermented & Dried"
                },
                new Product
                {
                    Id = 12,
                    Name = "Dark Chocolate Bar",
                    Origin = "Cusco, Peru",
                    Type = "Cacao",
                    Price = 9.21m,
                    Unit = "100g",
                    Rating = 5.0m,
                    ImageUrl = "/Media/Cacao/chocolate_bar.webp",
                    Description = "70% cacao dark chocolate bar made from premium Peruvian cacao. Smooth and rich with complex flavors.",
                    FlavorProfile = "Rich, Bitter, Fruity",
                    ProcessingMethod = "Fermented & Dried"
                },
                new Product
                {
                    Id = 13,
                    Name = "Chuncho Cacao",
                    Origin = "Cusco, Peru",
                    Type = "Cacao",
                    Price = 25.00m,
                    Unit = "1kg",
                    Rating = 5.0m,
                    ImageUrl = "/Media/Cacao/cacao_llama.webp",
                    Description = "Delicate and balanced with notes of woody cognac, cinnamon, sweet wine, and toffee. Native to Cusco's Amazonian valleys.",
                    FlavorProfile = "Vanilla, Green Tea, Cognac",
                    ProcessingMethod = "Fermented & Dried"
                },
                new Product
                {
                    Id = 14,
                    Name = "Piura White Cacao",
                    Origin = "Piura, Peru",
                    Type = "Cacao",
                    Price = 30.00m,
                    Unit = "1kg",
                    Rating = 4.9m,
                    ImageUrl = "/Media/Piura_White_White_Jewel.webp",
                    Description = "Known as 'the white jewel' for its exceptional aroma and low acidity. The beans are pale pink.",
                    FlavorProfile = "Delicate, Floral, Low Acidity",
                    ProcessingMethod = "Fermented & Dried"
                },
                new Product
                {
                    Id = 15,
                    Name = "Criollo Cacao",
                    Origin = "Amazonian, Peru",
                    Type = "Cacao",
                    Price = 35.00m,
                    Unit = "1kg",
                    Rating = 5.0m,
                    ImageUrl = "/Media/Cacao/Llama_Cacao_paper.webp",
                    Description = "The 'King of Cacao.' Delicate and complex with low bitterness. Subtle notes of red fruits and floral tones.",
                    FlavorProfile = "Red Fruits, Floral, Delicate",
                    ProcessingMethod = "Fermented & Dried"
                },
                new Product
                {
                    Id = 16,
                    Name = "Trinitario Cacao",
                    Origin = "Various Regions, Peru",
                    Type = "Cacao",
                    Price = 28.00m,
                    Unit = "1kg",
                    Rating = 4.8m,
                    ImageUrl = "/Media/Trinitario Cacao, the hybrid variety.webp",
                    Description = "A hybrid of Criollo and Forastero. Combines the smoothness of Criollo with the rich flavor of Forastero.",
                    FlavorProfile = "Balanced, Smooth, Rich",
                    ProcessingMethod = "Fermented & Dried"
                },
                new Product
                {
                    Id = 17,
                    Name = "Forastero Cacao",
                    Origin = "Amazon, Peru",
                    Type = "Cacao",
                    Price = 20.00m,
                    Unit = "1kg",
                    Rating = 4.6m,
                    ImageUrl = "/Media/Forastero Cacao, the robust Amazonian variety.webp",
                    Description = "Known for its bold, earthy, and strong cocoa taste. A resilient key ingredient in premium blends.",
                    FlavorProfile = "Bold, Earthy, Strong",
                    ProcessingMethod = "Fermented & Dried"
                },
                new Product
                {
                    Id = 101,
                    Name = "Hario V60",
                    Origin = "Japan",
                    Type = "Equipment",
                    Price = 24.99m,
                    Unit = "piece",
                    Rating = 4.8m,
                    ImageUrl = "/Media/Shop/Hario-V60_02-GlassBlack-v3.webp",
                    Description = "Glass pour-over coffee maker",
                    FlavorProfile = "",
                    ProcessingMethod = ""
                },
                new Product
                {
                    Id = 102,
                    Name = "AeroPress - Blue",
                    Origin = "USA",
                    Type = "Equipment",
                    Price = 39.99m,
                    Unit = "piece",
                    Rating = 4.9m,
                    ImageUrl = "/Media/Shop/aeropress_blue.webp",
                    Description = "Premium coffee press - Blue edition",
                    FlavorProfile = "",
                    ProcessingMethod = ""
                },
                new Product
                {
                    Id = 103,
                    Name = "AeroPress - Green",
                    Origin = "USA",
                    Type = "Equipment",
                    Price = 39.99m,
                    Unit = "piece",
                    Rating = 4.7m,
                    ImageUrl = "/Media/Shop/aeropress_green.webp",
                    Description = "Premium coffee press - Green edition",
                    FlavorProfile = "",
                    ProcessingMethod = ""
                },
                new Product
                {
                    Id = 104,
                    Name = "AeroPress - Grey",
                    Origin = "USA",
                    Type = "Equipment",
                    Price = 39.99m,
                    Unit = "piece",
                    Rating = 4.6m,
                    ImageUrl = "/Media/Shop/aeropress_grey.webp",
                    Description = "Premium coffee press - Grey edition",
                    FlavorProfile = "",
                    ProcessingMethod = ""
                }
            };
        }
    }
}
