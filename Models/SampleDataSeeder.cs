using Microsoft.AspNetCore.Identity;
using TheBestBean.Data;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Services;

namespace TheBestBean.Models
{
    public static class SampleDataSeeder
    {
        private static TheBestBeanContext? _context;

        public static async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Ensure Admin role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Also make sure all existing users have the Admin role so the user doesn't get locked out
            var users = await userManager.Users.ToListAsync();
            foreach (var u in users)
            {
                if (!await userManager.IsInRoleAsync(u, "Admin"))
                {
                    await userManager.AddToRoleAsync(u, "Admin");
                }
            }

            var adminEmail = "admin@purplebeancoffee.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newAdmin, "AdminPass123!");
                await userManager.AddToRoleAsync(newAdmin, "Admin");
            }
        }
    
        public static async Task SeedSampleDataAsync(TheBestBeanContext context)
        {
            _context = context;

            // Removed early return to ensure all entities are seeded if missing
            
            if (!_context.Experiences.Any(e => e.Category == "Expedition"))
            {
                _context.Experiences.AddRange(
                    new Experience {
                        Title = "Inkawasi Farm Tour",
                        Category = "Expedition",
                        Location = "CUSCO",
                        Month = "OCT",
                        Difficulty = "INTERMEDIATE",
                        Description = "Visit the Inkawasi cooperative and see the origin of our SL09 beans. Walk through the fields, meet the producers, and participate in the harvest process.",
                        ImageUrl = "https://images.unsplash.com/photo-1611162458324-aae1eb4129a4?auto=format&fit=crop&w=1200&q=80",
                        Price = 150,
                        Duration = "1 DAY",
                        Syllabus = new List<string> { "Morning departure from Cusco", "Farm tour & harvest", "Lunch with producers", "Return to Cusco" }
                    },
                    new Experience {
                        Title = "Huayopata Harvest Immersion",
                        Category = "Expedition",
                        Location = "LA CONVENCIÓN",
                        Month = "NOV",
                        Difficulty = "ADVANCED",
                        Description = "A deep dive into the processing methods of Huayopata Estate. Participate in washing, fermenting, and drying high-altitude Geisha coffee.",
                        ImageUrl = "https://images.unsplash.com/photo-1581007871115-f14bc016e0a4?auto=format&fit=crop&w=1200&q=80",
                        Price = 300,
                        Duration = "2 DAYS",
                        Syllabus = new List<string> { "Arrival & introduction", "Harvest & pulping", "Fermentation monitoring & drying", "Cupping the previous harvest" }
                    }
                );
                await _context.SaveChangesAsync();
            }



            // CMS SEEDING
            if (!_context.SiteContent.Any())
            {
                _context.SiteContent.AddRange(
                    new SiteContent { Key = "GreenBeans_Title", Value = "Flavor<br>Explorer.", Page = "GreenBeans" },
                    new SiteContent { Key = "GreenBeans_Subtitle", Value = "02 // LABORATORY ANALYSIS", Page = "GreenBeans" },
                    new SiteContent { Key = "GreenBeans_SensorySpecs", Value = "Sensory & Technical Specs", Page = "GreenBeans" },
                    new SiteContent { Key = "Tour_ReserveButton", Value = "Reserve Position", Page = "Tour" },
                    new SiteContent { Key = "Tour_DepositText", Value = "Requires a 50% deposit to secure equipment.", Page = "Tour" },
                    new SiteContent { Key = "Home_H1", Value = "Purple Bean Coffee", Page = "Index" }
                );
                await _context.SaveChangesAsync();
            }

            if (!_context.FlavorZones.Any())
            {
                _context.FlavorZones.AddRange(
                    new FlavorZone { Id = "fruity_citric", Name = "Citrus & Berries", Category = "Bright & Fruity", Description = "Driven by organic acids and vibrant fruit notes. Characterized by crisp brightness and delicate sweetness.", MarkersJson = "[\"Citric Acid\", \"Pineapple\", \"Orange\", \"Apple\", \"Floral\"]", Color = "#FFB800", BaryTargetJson = "{\"t\": 0.85, \"br\": 0.05, \"bl\": 0.1}", SpecsJson = "{\"roast\": [5, 20], \"body\": [10, 30], \"aftertaste\": [5, 25]}", RecommendationTitle = "Ethiopia Yirgacheffe", RecommendationOrigin = "Washed Process // Africa", RecommendationRoast = "Light Roast", RecommendationDesc = "A brilliantly bright washed coffee featuring sparkling acidity, bergamot, and crisp peach." },
                    new FlavorZone { Id = "floral_sweet", Name = "Floral Aromatics", Category = "Bright & Fruity", Description = "Highly volatile, delicate aromatics experienced primarily retro-nasally. Botanical perfumes and vanilla-like sweetness.", MarkersJson = "[\"Jasmine\", \"Vanilla\", \"Honey\", \"Chamomile\"]", Color = "#FF4D85", BaryTargetJson = "{\"t\": 0.5, \"br\": 0.1, \"bl\": 0.4}", SpecsJson = "{\"roast\": [10, 25], \"body\": [15, 35], \"aftertaste\": [20, 40]}", RecommendationTitle = "Panama Boquete Geisha", RecommendationOrigin = "Washed Process // Central America", RecommendationRoast = "Light Roast", RecommendationDesc = "World-renowned for its perfume-like aromatics. Expect intense jasmine and a delicate vanilla sweetness." },
                    new FlavorZone { Id = "alcohol_fermented", Name = "Fermented & Winey", Category = "Funky & Boozy", Description = "Complex, pungent, and slightly boozy. Results from extended microbial fermentation during processing.", MarkersJson = "[\"Whiskey\", \"Winey\", \"Overripe Fruit\", \"Tropical\"]", Color = "#8A2BE2", BaryTargetJson = "{\"t\": 0.3, \"br\": 0.6, \"bl\": 0.1}", SpecsJson = "{\"roast\": [30, 50], \"body\": [70, 90], \"aftertaste\": [75, 95]}", RecommendationTitle = "Colombia Cauca Anaerobic", RecommendationOrigin = "Anaerobic Natural // South America", RecommendationRoast = "Light-Med Roast", RecommendationDesc = "A highly experimental microlot with heavy boozy notes of aged whiskey, dark cherry, and wine-like acidity." },
                    new FlavorZone { Id = "spices_pungent", Name = "Warm Spices", Category = "Funky & Boozy", Description = "Warm, sharp, and aromatic. Driven by dry brown spices and a robust, sometimes peppery finish.", MarkersJson = "[\"Cinnamon\", \"Clove\", \"Nutmeg\", \"Pepper\"]", Color = "#D11141", BaryTargetJson = "{\"t\": 0.1, \"br\": 0.8, \"bl\": 0.1}", SpecsJson = "{\"roast\": [60, 80], \"body\": [60, 85], \"aftertaste\": [70, 90]}", RecommendationTitle = "Sumatra Lintong", RecommendationOrigin = "Wet-Hulled // Indonesia", RecommendationRoast = "Med-Dark Roast", RecommendationDesc = "Heavy bodied and earthy. Dominated by cedar, warm cinnamon, and a lingering clove finish on the palate." },
                    new FlavorZone { Id = "nutty_cocoa", Name = "Nutty & Chocolate", Category = "Rich & Chocolatey", Description = "Decadent and heavy-bodied. Defined by the comforting familiarity of roasted nuts and deep chocolate richness.", MarkersJson = "[\"Dark Chocolate\", \"Cacao\", \"Hazelnut\", \"Almond\"]", Color = "#5D4037", BaryTargetJson = "{\"t\": 0.1, \"br\": 0.1, \"bl\": 0.8}", SpecsJson = "{\"roast\": [75, 95], \"body\": [80, 95], \"aftertaste\": [55, 80]}", RecommendationTitle = "Brazil Minas Gerais", RecommendationOrigin = "Natural Process // South America", RecommendationRoast = "Dark Roast", RecommendationDesc = "A comforting classic. Extremely low acidity with a thick body, delivering intense dark chocolate and roasted hazelnut." },
                    new FlavorZone { Id = "sweet_brown_sugar", Name = "Caramel & Syrup", Category = "Rich & Chocolatey", Description = "Rich, comforting sweetness resulting from caramelization during roasting. Highly viscous mouthfeel.", MarkersJson = "[\"Maple Syrup\", \"Caramelized\", \"Brown Sugar\", \"Molasses\"]", Color = "#FF6B00", BaryTargetJson = "{\"t\": 0.2, \"br\": 0.3, \"bl\": 0.5}", SpecsJson = "{\"roast\": [40, 60], \"body\": [50, 75], \"aftertaste\": [45, 65]}", RecommendationTitle = "Guatemala Antigua", RecommendationOrigin = "Washed Process // Central America", RecommendationRoast = "Medium Roast", RecommendationDesc = "Incredibly structured and sweet. Coats the palate like maple syrup, finishing with clear notes of baked apple." },
                    new FlavorZone { Id = "balanced_center", Name = "Structural Equilibrium", Category = "Balanced Blend", Description = "The structural center point. A complex harmony capturing bright acidity, comforting sweetness, and robust body.", MarkersJson = "[\"Well-rounded\", \"Complex\", \"Structured\", \"Harmonious\"]", Color = "#1D4C3A", BaryTargetJson = "{\"t\": 0.33, \"br\": 0.33, \"bl\": 0.33}", SpecsJson = "{\"roast\": [45, 55], \"body\": [45, 55], \"aftertaste\": [45, 55]}", RecommendationTitle = "Cusco City Center Blend", RecommendationOrigin = "Washed & Natural // Mixed Origin", RecommendationRoast = "Medium Roast", RecommendationDesc = "Our signature laboratory blend. Harmonious, balancing fruit-forward brightness with deep chocolate structure." }
                );
                await _context.SaveChangesAsync();
            }


            // Seed Countries
            var peru = new OriginCountry { Name = "Peru" };
            var colombia = new OriginCountry { Name = "Colombia" };

            _context.OriginCountry.AddRange(peru, colombia);

            // Seed Regions
            var cusco = new CoffeeRegion { Name = "La Convención, Cusco" };
            var junin = new CoffeeRegion { Name = "Junín" };
            var huila = new CoffeeRegion { Name = "Huila [Pitalito]" };
            var narino = new CoffeeRegion { Name = "Nariño" };

            _context.CoffeeRegion.AddRange(cusco, junin, huila, narino);

            // Seed Farms
            // Peru Farms
            var inkawasi = new CoffeeFarm { Name = "Inkawasi" };
            var huayopata = new CoffeeFarm { Name = "Huayopata Estate" };
            // Colombia Farms
            var monteblanco = new CoffeeFarm { Name = "Monteblanco" };
            var elProgreso = new CoffeeFarm { Name = "El Progreso" };
            var buesaco = new CoffeeFarm { Name = "Minifundio Buesaco" };
            var ccelc = new CoffeeFarm { Name = "CCELC Cooperative" };

            _context.CoffeeFarm.AddRange(inkawasi, huayopata, monteblanco, elProgreso, buesaco, ccelc);

            await _context.SaveChangesAsync();

            // Create Coffee Beans
            if (!_context.CoffeeBean.Any())
            {
                // PERU BEANS
            var superHighland = new CoffeeBean
            {
                Name = "Super Highland [SL09 / Inca Geisha]",
                FlavorProfile = "Black currant, savory, complex acidity",
                Rating = 4.8m,
                ProcessingMethod = "Washed",
                Producer = "Oscar Vilches y Lisbet Mariño",
                ProducerDescription = "SL09 is our flagship coffee, grown by the Huadquiña cooperative in La Convención, Cusco. This cooperative consists of 300+ smallholder farmers dedicated to organic practices and preserving traditional Peruvian coffee varieties. Currently in stock and available for both local sale and international export.",
                ProcessingDescription = "SL09 is fully washed and fermented for 24-36 hours, then carefully dried on raised beds. This processing method results in a clean, bright cup that highlights the chocolate and caramel notes characteristic of La Convención coffees. The beans are then sorted and prepared for export or local roasting.",
                OriginDescription = "SL09 comes from La Convención province in Cusco, Peru's premier coffee-producing region. This area stretches from the high Andes to the edge of the Amazon rainforest, producing some of the world's finest organic coffee at altitudes between 1000-1600 meters.",
                Variety = "SL09 / Inca Geisha",
                Altitude = "2230 msnm",
                ImageUrl = "/Media/Green_bean.svg",
                ProducerImageUrl = "https://images.unsplash.com/photo-1596627689932-349099c26d7c?q=80&w=1600&auto=format&fit=crop",
                MapImageUrl = "/Media/LaConvencionMap.svg",
                CoffeeFarm = inkawasi,
                CoffeeRegion = cusco,
                OriginCountry = peru
            };

            var num13 = new CoffeeBean
            {
                Name = "Number 13 [Geisha]",
                FlavorProfile = "Jasmine, lemongrass, bergamot",
                Rating = 4.9m,
                ProcessingMethod = "Washed",
                Producer = "Oscar Vilches",
                ProducerDescription = "Grown by smallholder farmers in the Cusco region, particularly the Huadquiña cooperative and other local cooperatives. These farmers are dedicated to organic practices, sustainable farming, and preserving traditional Peruvian coffee varieties. Each batch represents the hard work of families who have cultivated coffee in these high-altitude regions for generations.",
                ProcessingDescription = "This Number 13 [Geisha] is fully washed and fermented for 24 hours. The result is a clean cup that highlights the terroir of the Cusco region.",
                OriginDescription = "Explore the Cusco region where this coffee is grown. The high-altitude farms, diverse microclimates, and traditional farming methods create exceptional quality coffee with unique flavor profiles.",
                Variety = "Geisha",
                Altitude = "2200 msnm",
                ImageUrl = "/Media/Green_bean.svg",
                ProducerImageUrl = "https://images.unsplash.com/photo-1596627689932-349099c26d7c?q=80&w=1600&auto=format&fit=crop",
                MapImageUrl = "/Media/LaConvencionMap.svg",
                CoffeeFarm = inkawasi,
                CoffeeRegion = cusco,
                OriginCountry = peru
            };

            var papi = new CoffeeBean
            {
                Name = "Regular Papi [Bourbon]",
                FlavorProfile = "Pink lemonade, floral, silky body",
                Rating = 4.7m,
                ProcessingMethod = "Washed",
                Producer = "Rodrigo Sanchez",
                ProducerDescription = "Grown by smallholder farmers in the Cusco region, dedicated to organic practices and preserving traditional Peruvian coffee varieties.",
                ProcessingDescription = "This Regular Papi [Bourbon] is processed using traditional methods from the Cusco region, carefully selected to bring out the unique characteristics of Peruvian coffee.",
                OriginDescription = "Explore the Cusco region where this coffee is grown. The high-altitude farms, diverse microclimates, and traditional farming methods create exceptional quality coffee with unique flavor profiles.",
                Variety = "Bourbon",
                Altitude = "1700 msnm",
                ImageUrl = "/Media/Green_bean.svg",
                ProducerImageUrl = "https://images.unsplash.com/photo-1596627689932-349099c26d7c?q=80&w=1600&auto=format&fit=crop",
                MapImageUrl = "/Media/LaConvencionMap.svg",
                CoffeeFarm = monteblanco,
                CoffeeRegion = huila,
                OriginCountry = colombia
            };

            var punch = new CoffeeBean
            {
                Name = "Punch [Geisha]",
                FlavorProfile = "Floral, peach, tea-like",
                Rating = 4.8m,
                ProcessingMethod = "Washed",
                Producer = "Teodocia Alvarez",
                ProducerDescription = "Grown by smallholder farmers in the Cusco region, particularly the Huadquiña cooperative and other local cooperatives. These farmers are dedicated to organic practices, sustainable farming, and preserving traditional Peruvian coffee varieties. Each batch represents the hard work of families who have cultivated coffee in these high-altitude regions for generations.",
                ProcessingDescription = "This Punch [Geisha] is fully washed and fermented for 24 hours. The result is a clean cup that highlights the terroir of the Cusco region.",
                OriginDescription = "Explore the Cusco region where this coffee is grown. The high-altitude farms, diverse microclimates, and traditional farming methods create exceptional quality coffee with unique flavor profiles.",
                Variety = "Geisha",
                Altitude = "1850 msnm",
                ImageUrl = "/Media/Green_bean.svg",
                ProducerImageUrl = "https://images.unsplash.com/photo-1596627689932-349099c26d7c?q=80&w=1600&auto=format&fit=crop",
                MapImageUrl = "/Media/LaConvencionMap.svg",
                CoffeeFarm = huayopata,
                CoffeeRegion = cusco,
                OriginCountry = peru
            };

            _context.CoffeeBean.AddRange(superHighland, num13, papi, punch);

            await _context.SaveChangesAsync();
            }
            if (!_context.Experiences.Any(e => e.Category == "Urban Workshops"))
            {
                _context.Experiences.AddRange(
                    new Experience
                    {
                        Title = "Latte Art Masterclass",
                        Category = "Urban Workshops",
                        Location = "Cusco City Center",
                        Duration = "3 Hours",
                        Month = "August",
                        Description = "Learn the fundamentals of milk texturing and latte art pouring from our award-winning baristas. Includes unlimited milk and coffee practice.",
                        Price = 45.00M,
                        ImageUrl = "https://images.unsplash.com/photo-1541167760496-1628856ab772?auto=format&fit=crop&w=600&q=80"
                    },
                    new Experience
                    {
                        Title = "Advanced Sensory Evaluation",
                        Category = "Urban Workshops",
                        Location = "Cusco City Center",
                        Duration = "2 Hours",
                        Month = "September",
                        Description = "A deep dive into SCA cupping protocols, defect identification, and advanced palate development. Ideal for aspiring coffee professionals.",
                        Price = 75.00M,
                        ImageUrl = "https://images.unsplash.com/photo-1497935586351-b67a49e012bf?auto=format&fit=crop&w=600&q=80"
                    },
                    new Experience
                    {
                        Title = "Home Brewing 101",
                        Category = "Urban Workshops",
                        Location = "Cusco City Center",
                        Duration = "2.5 Hours",
                        Month = "October",
                        Description = "Master the V60, AeroPress, and French Press. Understand extraction variables and how to troubleshoot your morning cup.",
                        Price = 35.00M,
                        ImageUrl = "https://images.unsplash.com/photo-1495474472207-464a4f54e156?auto=format&fit=crop&w=600&q=80"
                    },
                    new Experience
                    {
                        Title = "Espresso Fundamentals",
                        Category = "Urban Workshops",
                        Location = "CUSCO",
                        Month = "SEP",
                        Difficulty = "ADVANCED",
                        Description = "Detailed cupping sessions comparing rare lots. Discuss senses, share notes, and calibrate your palate alongside professionals.",
                        ImageUrl = "https://images.unsplash.com/photo-1611162458324-aae1eb4129a4?auto=format&fit=crop&w=1200&q=80",
                        Price = 55
                    },
                    new Experience {
                        Title = "SCA Flavor Profiling",
                        Category = "Urban Workshops",
                        Location = "CUSCO",
                        Month = "SEP",
                        Difficulty = "INTERMEDIATE",
                        Description = "Train your palate using industry-standard protocols. Blind-taste, score, and chart acidity, body, and tasting notes of exotic Peruvian varietals.",
                        ImageUrl = "https://images.unsplash.com/photo-1581007871115-f14bc016e0a4?auto=format&fit=crop&w=1200&q=80",
                        Price = 75
                    },
                    new Experience {
                        Title = "Espresso Machines Lab",
                        Category = "Urban Workshops",
                        Location = "CUSCO",
                        Month = "SEP",
                        Difficulty = "INTERMEDIATE",
                        Description = "Master the craft of precision extraction. Refine your volumetric parameters and sensory dynamics to build the perfect cup.",
                        ImageUrl = "/images/brewer_withGrinds.webp",
                        Price = 65
                    },
                    new Experience {
                        Title = "Roasting Foundations",
                        Category = "Urban Workshops",
                        Location = "SPECIAL",
                        Month = "OCT",
                        Difficulty = "ADVANCED",
                        Description = "Official SCA Sensory Skills Foundation certification. Includes rigorous blind triangulation and professional cupping protocol exams.",
                        ImageUrl = "https://images.unsplash.com/photo-1514432324607-a09d9b4aefdd?auto=format&fit=crop&w=1200&q=80",
                        Price = 250,
                        Tag = "GUEST EVENT"
                    }
                );
                await _context.SaveChangesAsync();
            }

        }

        public static async Task FixDataAsync(TheBestBeanContext context)
        {
            _context = context;
            
            var colombia = await _context.OriginCountry.FirstOrDefaultAsync(c => c.Name == "Colombia");
            if (colombia != null)
            {
                colombia.Name = "Colombia";
                _context.OriginCountry.Update(colombia);
            }
            
            var conflictingKeys = new[] { 
                "GreenBeans_Title", "Home_Section1_Title", "Home_Labs_Title", "Home_Subs_Title",
                "Experiences_UrbanLabs_Title", "Experiences_Expeditions_Title" 
            };
            var conflictingContent = await _context.SiteContent.Where(s => conflictingKeys.Contains(s.Key)).ToListAsync();
            if (conflictingContent.Any())
            {
                _context.SiteContent.RemoveRange(conflictingContent);
            }

            // 1.5 Update Regions
            var huila = await _context.CoffeeRegion.FirstOrDefaultAsync(r => r.Name == "Huila");
            if (huila != null)
            {
                huila.Name = "Huila [Pitalito]";
                _context.CoffeeRegion.Update(huila);
            }

            var narino = await _context.CoffeeRegion.FirstOrDefaultAsync(r => r.Name == "Nariño");
            if (narino == null) 
            {
                narino = new CoffeeRegion { Name = "Nariño" };
                _context.CoffeeRegion.Add(narino);
            }

            // 2. Remove non-Peru/Colombia entries
            var allowedCountries = new[] { "Peru", "Colombia" };
            
            var beansToRemove = await _context.CoffeeBean
                .Include(b => b.OriginCountry)
                .Where(b => !allowedCountries.Contains(b.OriginCountry.Name))
                .ToListAsync();
            
            if (beansToRemove.Any())
            {
                _context.CoffeeBean.RemoveRange(beansToRemove);
            }

            var countriesToRemove = await _context.OriginCountry
                .Where(c => !allowedCountries.Contains(c.Name))
                .ToListAsync();

            if (countriesToRemove.Any())
            {
                _context.OriginCountry.RemoveRange(countriesToRemove);
            }

            // Also clean up regions/farms if possible, but beans/countries is the visible part.
            await SeedNewOriginLotsAsync();
            await ApplyAngelCatarataPhotosAsync();
            await ApplyCoffeeRetailAsync();
            await ApplyOriginExpeditionsAsync();
            await ApplyCuscoWorkshopRosterAsync();

            // Let's just save changes now to be safe.
            await _context.SaveChangesAsync();
        }

        private static async Task SeedNewOriginLotsAsync()
        {
            var newLotNames = new[]
            {
                "Cajamarca SL28 [72h]",
                "La Catarata [Geisha]",
                "Cajamarca [Bourbon]",
                "Cusco [Bourbon]",
                "Cajamarca [Marsellesa]"
            };
            var existing = await _context.CoffeeBean
                .Where(b => newLotNames.Contains(b.Name))
                .Select(b => b.Name)
                .ToListAsync();
            if (existing.Count == newLotNames.Length) return;

            var peru = await _context.OriginCountry.FirstOrDefaultAsync(c => c.Name == "Peru");
            if (peru == null)
            {
                peru = new OriginCountry { Name = "Peru" };
                _context.OriginCountry.Add(peru);
                await _context.SaveChangesAsync();
            }

            async Task<CoffeeRegion> RegionAsync(string name)
            {
                var region = await _context.CoffeeRegion.FirstOrDefaultAsync(r => r.Name == name);
                if (region != null) return region;
                region = new CoffeeRegion { Name = name };
                _context.CoffeeRegion.Add(region);
                await _context.SaveChangesAsync();
                return region;
            }

            async Task<CoffeeFarm> FarmAsync(string name)
            {
                var farm = await _context.CoffeeFarm.FirstOrDefaultAsync(f => f.Name == name);
                if (farm != null) return farm;
                farm = new CoffeeFarm { Name = name };
                _context.CoffeeFarm.Add(farm);
                await _context.SaveChangesAsync();
                return farm;
            }

            var cajamarca = await RegionAsync("Jaén, Cajamarca");
            var cusco = await RegionAsync("La Convención, Cusco");
            var laCatarata = await FarmAsync("La Catarata");
            var cajamarcaHighlands = await FarmAsync("Cajamarca Highlands");
            var convencion = await FarmAsync("Inkawasi");

            var lots = new List<CoffeeBean>();

            if (!existing.Contains("Cajamarca SL28 [72h]"))
            {
                lots.Add(new CoffeeBean
                {
                    Name = "Cajamarca SL28 [72h]",
                    FlavorProfile = "Blackcurrant, red berries, tropical fruit",
                    FlavorProfileES = "Grosella negra, frutos rojos, fruta tropical",
                    Rating = 4.8m,
                    ScaScore = 87.50m,
                    BasePriceUSD = 32.00m,
                    BasePricePEN = 120.00m,
                    ProcessingMethod = "Washed",
                    Producer = "Cajamarca smallholders",
                    Variety = "SL28",
                    Altitude = "1700–1950 msnm",
                    ImageUrl = "/Media/Green_bean.svg",
                    ProducerDescription = "SL28 is a Kenyan selection now planted in Peru’s northern highlands. This lot comes from Cajamarca, where cool nights and high altitude slow cherry ripening and concentrate the variety’s blackcurrant character.",
                    ProducerDescriptionES = "SL28 es una selección keniana ahora cultivada en la sierra norte del Perú. Este lote proviene de Cajamarca, donde las noches frías y la altura alargan la maduración y concentran el carácter a grosella negra de la variedad.",
                    ProcessingDescription = "Ripe cherries are sorted, then held for a 72-hour fermentation before drying. The extended fermentation builds tropical fruit and berry intensity on top of SL28’s classic blackcurrant acidity.",
                    ProcessingDescriptionES = "Las cerezas maduras se seleccionan y fermentan durante 72 horas antes del secado. La fermentación prolongada suma intensidad a fruta tropical y frutos rojos sobre la acidez clásica a grosella negra del SL28.",
                    OriginDescription = "Cajamarca — especially Jaén and San Ignacio — is Peru’s competition corridor: farms from 1,500 to over 2,200 msnm, with washed and experimental fermentations that show clean citric structure and fruit.",
                    OriginDescriptionES = "Cajamarca — sobre todo Jaén y San Ignacio — es el corredor de competencia del Perú: fincas de 1.500 a más de 2.200 msnm, con lavados y fermentaciones experimentales de estructura cítrica y fruta limpia.",
                    CoffeeFarm = cajamarcaHighlands,
                    CoffeeRegion = cajamarca,
                    OriginCountry = peru
                });
            }

            if (!existing.Contains("La Catarata [Geisha]"))
            {
                lots.Add(new CoffeeBean
                {
                    Name = "La Catarata [Geisha]",
                    FlavorProfile = "Citrus, lemongrass, lavender, stone fruit",
                    FlavorProfileES = "Cítricos, lemongrass, lavanda, fruta de hueso",
                    Rating = 5.0m,
                    ScaScore = 90.64m,
                    BasePriceUSD = 48.00m,
                    BasePricePEN = 180.00m,
                    ProcessingMethod = "Washed",
                    Producer = "Ángel Antonio Manosalva Palomino",
                    Variety = "Geisha",
                    Altitude = "1950 msnm",
                    ImageUrl = "/Media/producers/angel-cajamarca/catarata-sign.jpg?v=4",
                    ProducerImageUrl = "/Media/producers/angel-cajamarca/angel-portrait.jpg?v=4",
                    ProducerDescription = "Ángel Antonio Manosalva Palomino farms La Catarata and La Pomarrosa with his wife Luz Marita González Rojas in La Cascarilla, Jaén, Cajamarca. A Cenfrocafé member, he took over a neglected 1.75-hectare plot in 2020, completed ownership in 2023, and replanted nearly 8,000 Geisha trees. In 2025 his washed Geisha won Taza de Excelencia Perú with 90.64 points — first among ~300 samples.",
                    ProducerDescriptionES = "Ángel Antonio Manosalva Palomino cultiva La Catarata y La Pomarrosa junto a su esposa Luz Marita González Rojas en La Cascarilla, Jaén, Cajamarca. Socio de Cenfrocafé, tomó un predio de 1,75 ha en 2020, consolidó la propiedad en 2023 y replantó cerca de 8.000 árboles de Geisha. En 2025 su Geisha lavado ganó la Taza de Excelencia Perú con 90,64 puntos.",
                    ProcessingDescription = "Fully washed Geisha. Ángel has spent seven years refining fermentations for a floral, clean cup. The winning 2025 lot was harvested around August and processed as a washed microlot from this tiny Jaén plot.",
                    ProcessingDescriptionES = "Geisha totalmente lavado. Ángel lleva siete años afinando fermentaciones para una taza floral y limpia. El lote ganador de 2025 se cosechó hacia agosto y se procesó como microlote lavado de esta parcela en Jaén.",
                    OriginDescription = "La Cascarilla sits in the mountains above Jaén, one of Cajamarca’s coffee hubs. At 1,950 msnm the diurnal swing and Andean soils give washed Geisha citrus, lemongrass, lavender, caramel, and stone fruit.",
                    OriginDescriptionES = "La Cascarilla está en las montañas sobre Jaén, uno de los centros cafetaleros de Cajamarca. A 1.950 msnm, el rango diurno y los suelos andinos dan al Geisha lavado cítricos, lemongrass, lavanda, caramelo y fruta de hueso.",
                    CoffeeFarm = laCatarata,
                    CoffeeRegion = cajamarca,
                    OriginCountry = peru
                });
            }

            if (!existing.Contains("Cajamarca [Bourbon]"))
            {
                lots.Add(new CoffeeBean
                {
                    Name = "Cajamarca [Bourbon]",
                    FlavorProfile = "Brown sugar, citrus, stone fruit",
                    FlavorProfileES = "Azúcar morena, cítricos, fruta de hueso",
                    Rating = 4.6m,
                    ScaScore = 86.50m,
                    BasePriceUSD = 26.00m,
                    BasePricePEN = 98.00m,
                    ProcessingMethod = "Washed",
                    Producer = "Cajamarca producers",
                    Variety = "Bourbon",
                    Altitude = "1600–2000 msnm",
                    ImageUrl = "/Media/Green_bean.svg",
                    ProducerDescription = "Bourbon sits on Cajamarca’s higher slopes in Jaén and San Ignacio, typically 1,600–2,000 msnm, beside Typica. Smallholders in cooperatives such as Cenfrocafé and APROCASSI deliver cherry to central mills for consistent washed lots.",
                    ProducerDescriptionES = "El Borbón ocupa las laderas altas de Cajamarca en Jaén y San Ignacio, entre 1.600 y 2.000 msnm, junto al Típica. Pequeños productores de cooperativas como Cenfrocafé y APROCASSI entregan cereza a molinos centrales para lotes lavados consistentes.",
                    ProcessingDescription = "Fully washed: depulped, fermented 18–36 hours, washed, and dried on patios or raised beds. The process keeps fruit influence off the cup so Bourbon’s brown-sugar sweetness and Cajamarca citric structure can read clearly.",
                    ProcessingDescriptionES = "Totalmente lavado: despulpado, fermentado 18–36 horas, lavado y secado en patios o camas africanas. El proceso deja fuera la fruta de la cereza para que brille el dulzor a azúcar morena del Borbón y la estructura cítrica de Cajamarca.",
                    OriginDescription = "Northern Cajamarca is Peru’s competition highlands. Dual Pacific/Amazon moisture and altitude produce clean, citric washed cups. Bourbon here adds body and brown-sugar sweetness under the region’s citrus and stone-fruit acidity.",
                    OriginDescriptionES = "El norte de Cajamarca es la sierra de competencia del Perú. La humedad del Pacífico y la Amazonía, más la altura, dan tazas lavadas cítricas y limpias. El Borbón aporta cuerpo y dulzor a azúcar morena bajo la acidez a cítricos y fruta de hueso.",
                    CoffeeFarm = cajamarcaHighlands,
                    CoffeeRegion = cajamarca,
                    OriginCountry = peru
                });
            }

            if (!existing.Contains("Cusco [Bourbon]"))
            {
                lots.Add(new CoffeeBean
                {
                    Name = "Cusco [Bourbon]",
                    FlavorProfile = "Floral, caramel, soft acidity",
                    FlavorProfileES = "Floral, caramelo, acidez suave",
                    Rating = 4.6m,
                    ScaScore = 86.25m,
                    BasePriceUSD = 26.00m,
                    BasePricePEN = 98.00m,
                    ProcessingMethod = "Washed",
                    Producer = "La Convención producers",
                    Variety = "Bourbon",
                    Altitude = "1600–2000 msnm",
                    ImageUrl = "/Media/Green_bean.svg",
                    ProducerDescription = "Bourbon plantings in Cusco are older and less common than in the north. They sit on steep La Convención slopes above about 1,600 msnm, often under native shade, with families processing cherry on-farm.",
                    ProducerDescriptionES = "Las plantaciones de Borbón en Cusco son más antiguas y menos comunes que en el norte. Están en laderas empinadas de La Convención sobre unos 1.600 msnm, a menudo bajo sombra nativa, con familias que procesan en finca.",
                    ProcessingDescription = "Fully washed after selective picking. Fermentation is typically 16–36 hours, then parchment is dried on raised beds. Some Cusco farms extend fermentation; this lot is a classic washed Bourbon to show the valley’s gentler, floral cup.",
                    ProcessingDescriptionES = "Totalmente lavado tras cosecha selectiva. La fermentación suele durar 16–36 horas y el pergamino se seca en camas africanas. Algunas fincas alargan la fermentación; este lote es un Borbón lavado clásico para mostrar la taza más floral del valle.",
                    OriginDescription = "Cusco’s coffee belt is La Convención — Quillabamba, Echarate, Vilcabamba — from the high Andes toward the Amazon. Cups here are softer and more floral than Cajamarca: slower Andean ripening, less centralized mills, and Bourbon sweetness as caramel and florals.",
                    OriginDescriptionES = "El cinturón cafetero del Cusco es La Convención — Quillabamba, Echarate, Vilcabamba — de los Andes altos hacia la Amazonía. La taza es más suave y floral que en Cajamarca: maduración andina lenta, menos molinos centrales y el dulzor del Borbón como caramelo y florales.",
                    CoffeeFarm = convencion,
                    CoffeeRegion = cusco,
                    OriginCountry = peru
                });
            }

            if (!existing.Contains("Cajamarca [Marsellesa]"))
            {
                lots.Add(new CoffeeBean
                {
                    Name = "Cajamarca [Marsellesa]",
                    FlavorProfile = "Cacao, citrus, honey, stone fruit",
                    FlavorProfileES = "Cacao, cítricos, miel, fruta de hueso",
                    Rating = 4.7m,
                    ScaScore = 87.00m,
                    BasePriceUSD = 28.00m,
                    BasePricePEN = 105.00m,
                    ProcessingMethod = "Washed",
                    Producer = "Cajamarca producers",
                    Variety = "Marsellesa",
                    Altitude = "1400–1800 msnm",
                    ImageUrl = "/Media/Green_bean.svg",
                    ProducerDescription = "Marsellesa is a Sarchimor (Timor Hybrid 832/2 × Villa Sarchi) released by CIRAD-ECOM in 2009. Cenfrocafé and the Junta Nacional del Café promoted it in Peru for rust resistance without giving up cup quality. In Cajamarca it is planted from about 1,400–1,800 msnm in Jaén, San Ignacio, and Cutervo.",
                    ProducerDescriptionES = "Marsellesa es un Sarchimor (Timor Hybrid 832/2 × Villa Sarchi) liberado por CIRAD-ECOM en 2009. Cenfrocafé y la Junta Nacional del Café la impulsaron en el Perú por resistencia a la roya sin sacrificar taza. En Cajamarca se planta entre unos 1.400 y 1.800 msnm en Jaén, San Ignacio y Cutervo.",
                    ProcessingDescription = "Washed, in the Cajamarca mill style: depulped, fermented, washed clean, and dried. At altitude Marsellesa shows the chocolate-citrus cup World Coffee Research flags for this variety — cleaner than older Catimors.",
                    ProcessingDescriptionES = "Lavado, al estilo de los molinos de Cajamarca: despulpado, fermentado, lavado y secado. En altura la Marsellesa muestra la taza cacao-cítrica que World Coffee Research destaca — más limpia que los Catimor antiguos.",
                    OriginDescription = "Cajamarca has been renewing rust-hit Caturra and Typica plots with Marsellesa and H1. Tabaconas (San Ignacio) trials above 1,400 msnm showed good yield, rust resistance, and specialty cupping. Expect cacao, citrus, honey, and stone fruit.",
                    OriginDescriptionES = "Cajamarca renueva parcelas de Caturra y Típica afectadas por roya con Marsellesa y H1. Ensayos en Tabaconas (San Ignacio) sobre 1.400 msnm mostraron buen rendimiento, resistencia y taza de especialidad. Cacao, cítricos, miel y fruta de hueso.",
                    CoffeeFarm = cajamarcaHighlands,
                    CoffeeRegion = cajamarca,
                    OriginCountry = peru
                });
            }

            if (lots.Count > 0)
            {
                _context.CoffeeBean.AddRange(lots);
            }
        }

        private static async Task ApplyCoffeeRetailAsync()
        {
            var beans = await _context.CoffeeBean.Include(b => b.CoffeeRegion).ToListAsync();
            foreach (var bean in beans)
            {
                var pen = LocalPricing.CoffeeHundredGramsPen(bean.ScaScore);
                bean.BasePricePEN = pen;
                bean.BasePriceUSD = LocalPricing.CoffeeBagUsd(bean.ScaScore, "100g");

                var region = bean.CoffeeRegion?.Name ?? "";
                var name = bean.Name ?? "";
                var isCajamarca = region.Contains("Cajamarca", StringComparison.OrdinalIgnoreCase)
                    || name.Contains("Cajamarca", StringComparison.OrdinalIgnoreCase)
                    || name.Contains("Catarata", StringComparison.OrdinalIgnoreCase);
                if (isCajamarca)
                {
                    bean.MapImageUrl = "/images/maps/cajamarca-jaen.svg";
                }
                else if (region.Contains("Convención", StringComparison.OrdinalIgnoreCase)
                    || region.Contains("Cusco", StringComparison.OrdinalIgnoreCase)
                    || name.Contains("Cusco", StringComparison.OrdinalIgnoreCase)
                    || name.Contains("SL09", StringComparison.OrdinalIgnoreCase)
                    || name.Contains("HIGHLAND", StringComparison.OrdinalIgnoreCase))
                {
                    bean.MapImageUrl = "/Media/LaConvencionMap.svg";
                }
            }
        }

        private static async Task ApplyAngelCatarataPhotosAsync()
        {
            var angel = await _context.CoffeeBean.FirstOrDefaultAsync(b => b.Name == "La Catarata [Geisha]");
            if (angel == null)
            {
                return;
            }

            var hero = "/Media/producers/angel-cajamarca/catarata-sign.jpg?v=4";
            var farmer = "/Media/producers/angel-cajamarca/angel-portrait.jpg?v=4";
            var changed = false;
            if (angel.ImageUrl != hero)
            {
                angel.ImageUrl = hero;
                changed = true;
            }
            if (angel.ProducerImageUrl != farmer)
            {
                angel.ProducerImageUrl = farmer;
                changed = true;
            }
            if (changed)
            {
                _context.CoffeeBean.Update(angel);
            }
        }

        private static async Task ApplyOriginExpeditionsAsync()
        {
            const string v = "?v=1";
            var santaGallery = new List<string>
            {
                $"/Media/tours/santa-teresa/10-visitor-and-producer.jpg{v}",
                $"/Media/tours/santa-teresa/01-cloud-forest-morning.jpg{v}",
                $"/Media/tours/santa-teresa/02-farm-walk.jpg{v}",
                $"/Media/tours/santa-teresa/03-coffee-trees.jpg{v}",
                $"/Media/tours/santa-teresa/04-cherries-on-branch.jpg{v}",
                $"/Media/tours/santa-teresa/05-harvest.jpg{v}",
                $"/Media/tours/santa-teresa/06-cloud-forest-canopy.jpg{v}",
                $"/Media/tours/santa-teresa/07-afternoon-farm.jpg{v}",
                $"/Media/tours/santa-teresa/08-valley-light.jpg{v}",
                $"/Media/tours/santa-teresa/09-hidroelectrica-route.jpg{v}",
                $"/Media/tours/santa-teresa/11-cup-at-the-farm.jpg{v}",
                $"/Media/tours/santa-teresa/12-ripe-cherries.jpg{v}",
                $"/Media/tours/santa-teresa/13-washed-parchment-drying.jpg{v}",
            };
            var cajamarcaGallery = new List<string>
            {
                $"/Media/tours/cajamarca-jaen/01-la-catarata-sign.jpg{v}",
                $"/Media/tours/cajamarca-jaen/02-farmer-angel.jpg{v}",
                $"/Media/tours/cajamarca-jaen/03-angel-in-the-plot.jpg{v}",
                $"/Media/tours/cajamarca-jaen/04-cherries-on-the-tree.jpg{v}",
                $"/Media/tours/cajamarca-jaen/05-brix-sweetness.jpg{v}",
                $"/Media/tours/cajamarca-jaen/06-measuring-ripeness.jpg{v}",
                $"/Media/tours/cajamarca-jaen/07-cherry-clusters.jpg{v}",
                $"/Media/tours/cajamarca-jaen/08-fruit-on-the-stem.jpg{v}",
                $"/Media/tours/cajamarca-jaen/09-ph-meter.jpg{v}",
                $"/Media/tours/cajamarca-jaen/10-process-check.jpg{v}",
                $"/Media/tours/cajamarca-jaen/11-fermentation.jpg{v}",
                $"/Media/tours/cajamarca-jaen/12-wet-mill.jpg{v}",
                $"/Media/tours/cajamarca-jaen/13-pulping-station.jpg{v}",
                $"/Media/tours/cajamarca-jaen/14-depulper-and-pulp.jpg{v}",
                $"/Media/tours/cajamarca-jaen/15-washing-channel.jpg{v}",
                $"/Media/tours/cajamarca-jaen/16-parchment-drying.jpg{v}",
                $"/Media/tours/cajamarca-jaen/17-dried-parchment.jpg{v}",
                $"/Media/tours/cajamarca-jaen/18-bagged-lots.jpg{v}",
                $"/Media/tours/cajamarca-jaen/19-angel-with-finished-beans.jpg{v}",
            };

            var santa = await _context.Experiences.FirstOrDefaultAsync(e => e.Id == 1 || e.Title.Contains("Inkawasi"));
            if (santa != null)
            {
                santa.Title = "Santa Teresa [Cloud Forest]";
                santa.TitleES = "Santa Teresa [Bosque de nubes]";
                santa.Category = "Expedition";
                santa.Location = "Santa Teresa";
                santa.Month = "Year-round";
                santa.Difficulty = "Intermediate";
                santa.Duration = "1 DAY";
                santa.Price = 150;
                santa.Tag = "MACHU PICCHU ROUTE";
                santa.ImageUrl = $"/Media/tours/santa-teresa/10-visitor-and-producer.jpg{v}";
                santa.Description = "Cloud-forest farm day on the Santa Teresa / Hidroeléctrica corridor most travelers use to reach Machu Picchu. Walk the plantation, pick ripe cherry, and see washed parchment drying with the family — then continue to Aguas Calientes.";
                santa.DescriptionES = "Día de finca en el bosque de nubes de Santa Teresa, en el corredor de Hidroeléctrica que usa la mayoría para llegar a Machu Picchu. Recorre el cafetal, cosecha cereza y ve el pergamino lavado secándose con la familia.";
                santa.LongDescription = "Santa Teresa sits in the cloud forest of La Convención, on the same road and rail corridor that takes you to Machu Picchu via Hidroeléctrica. This is the origin day that fits a trekking itinerary: leave Cusco (or stay in Santa Teresa), walk shade-grown plots, taste cherry at the tree, and watch washed parchment dry on raised beds. You meet the producers who pick and process the lot — not a show farm. Combine it with the walk or train into Aguas Calientes the same day or the next morning.";
                santa.LongDescriptionES = "Santa Teresa está en el bosque de nubes de La Convención, en el mismo corredor hacia Machu Picchu por Hidroeléctrica. Es el día de origen que encaja en un itinerario de trekking: caminas cafetales bajo sombra, pruebas la cereza en el árbol y ves el pergamino lavado en camas. Conoces a quienes cosechan y procesan el lote. Se combina con el tren o la caminata a Aguas Calientes.";
                santa.Syllabus = new List<string>
                {
                    "Cusco or Santa Teresa pickup — Hidroeléctrica corridor",
                    "Cloud-forest walk — trees, cherry, and shade canopy",
                    "Harvest and washed processing with the family",
                    "Parchment drying beds + cup at the farm",
                    "Continue toward Machu Picchu or return to Cusco"
                };
                santa.ProvidedEquipment = new List<string>
                {
                    "Farm walk with a producer",
                    "Harvest and processing demonstration",
                    "Lunch at the farm",
                    "Cup of the day’s lot"
                };
                santa.RequiredGear = new List<string>
                {
                    "Closed-toe shoes",
                    "Rain layer (cloud forest)",
                    "Sun hat"
                };
                santa.GalleryImages = santaGallery;
                _context.Experiences.Update(santa);
            }

            var cajamarca = await _context.Experiences.FirstOrDefaultAsync(e => e.Id == 2 || e.Title.Contains("Huayopata"));
            if (cajamarca != null)
            {
                cajamarca.Title = "La Catarata [Jaén, Cajamarca]";
                cajamarca.TitleES = "La Catarata [Jaén, Cajamarca]";
                cajamarca.Category = "Expedition";
                cajamarca.Location = "Jaén, Cajamarca";
                cajamarca.Month = "Harvest";
                cajamarca.Difficulty = "Intermediate";
                cajamarca.Duration = "2 DAYS";
                cajamarca.Price = 300;
                cajamarca.Tag = "CUP OF EXCELLENCE #1";
                cajamarca.ImageUrl = $"/Media/tours/cajamarca-jaen/01-la-catarata-sign.jpg{v}";
                cajamarca.Description = "Not a Cusco day trip — Jaén is in Cajamarca, northern Peru. Visit Ángel Manosalva at La Catarata: 2025 Cup of Excellence Perú first place, 90.64 points. Follow the lot from tree to parchment: Brix on cherry, pH in process, pulping, fermentation, dried bags.";
                cajamarca.DescriptionES = "No es un día desde Cusco: Jaén está en Cajamarca, en el norte del Perú. Visita a Ángel Manosalva en La Catarata, primer lugar Taza de Excelencia Perú 2025 (90,64). Recorres el lote del árbol al pergamino: Brix, pH, despulpado, fermentación y sacos secos.";
                cajamarca.LongDescription = "Ángel Antonio Manosalva Palomino farms La Catarata (and La Pomarrosa) in La Cascarilla above Jaén — a different province from Cusco, in Peru’s northern competition belt. In 2025 his washed Geisha took first at Taza de Excelencia Perú with 90.64 points. This expedition is the farm journey in order: the Catarata La Momia sign, Ángel in the plot, cherry on the tree, Brix for sweetness, pH at the mill, pulping and fermentation, then parchment drying and Ángel standing with the finished bags. Fly Lima–Jaén (or overnight bus); this is not a same-day return from Cusco.";
                cajamarca.LongDescriptionES = "Ángel Antonio Manosalva Palomino cultiva La Catarata (y La Pomarrosa) en La Cascarilla, sobre Jaén — otra región, el corredor de competencia del norte. En 2025 su Geisha lavado ganó la Taza de Excelencia Perú con 90,64 puntos. El recorrido sigue el lote: letrero de Catarata La Momia, Ángel en la parcela, cereza, Brix, pH, despulpado, fermentación, pergamino y los sacos terminados. Se vuela Lima–Jaén; no es un ida y vuelta desde Cusco.";
                cajamarca.Syllabus = new List<string>
                {
                    "Arrive Jaén (flight from Lima or overnight bus)",
                    "La Catarata — farm sign, plot walk with Ángel",
                    "Ripeness work — Brix on cherry, fruit on the tree",
                    "Mill — pH, pulping, fermentation, wash",
                    "Parchment drying and bagged lots — cup the CoE Geisha"
                };
                cajamarca.ProvidedEquipment = new List<string>
                {
                    "Farm and mill walk with Ángel",
                    "Brix and pH demonstration",
                    "Overnight in Jaén",
                    "Cup of the Cup of Excellence Geisha"
                };
                cajamarca.RequiredGear = new List<string>
                {
                    "Closed-toe shoes",
                    "Light rain jacket",
                    "Passport for domestic flights"
                };
                cajamarca.GalleryImages = cajamarcaGallery;
                _context.Experiences.Update(cajamarca);
            }

            var subtitle = await _context.SiteContent.FirstOrDefaultAsync(c => c.Key == "Experiences_Expeditions_Subtitle");
            if (subtitle != null)
            {
                subtitle.Value = "SANTA TERESA · CAJAMARCA";
            }
            else
            {
                _context.SiteContent.Add(new SiteContent
                {
                    Key = "Experiences_Expeditions_Subtitle",
                    Value = "SANTA TERESA · CAJAMARCA",
                    Page = "Experiences"
                });
            }
        }

        private static async Task ApplyCuscoWorkshopRosterAsync()
        {
            var v = "?v=1";
            var odarGallery = new List<string>
            {
                $"/Media/tours/odar/01-lab-cupping-table.jpg{v}",
                $"/Media/tours/odar/02-odar-cupping-spoon.jpg{v}",
                $"/Media/tours/odar/03-cupping-forms.jpg{v}",
                $"/Media/tours/odar/04-guided-cupping.jpg{v}",
                $"/Media/tours/odar/05-flavor-wheel.jpg{v}",
            };

            async Task<Experience> UpsertWorkshopAsync(string titleMatch, Experience values)
            {
                var row = await _context.Experiences.FirstOrDefaultAsync(e => e.Title.Contains(titleMatch));
                if (row == null)
                {
                    row = new Experience { Title = values.Title };
                    _context.Experiences.Add(row);
                }
                row.Title = values.Title;
                row.TitleES = values.TitleES;
                row.Category = "Urban Workshops";
                row.Location = values.Location;
                row.Month = values.Month;
                row.Difficulty = values.Difficulty;
                row.Duration = values.Duration;
                row.Price = values.Price;
                row.Tag = values.Tag;
                row.ImageUrl = values.ImageUrl;
                row.Description = values.Description;
                row.DescriptionES = values.DescriptionES;
                row.LongDescription = values.LongDescription;
                row.LongDescriptionES = values.LongDescriptionES;
                row.Syllabus = values.Syllabus;
                row.ProvidedEquipment = values.ProvidedEquipment;
                row.RequiredGear = values.RequiredGear;
                row.GalleryImages = values.GalleryImages;
                return row;
            }

            var cynthiaTastingGallery = new List<string>
            {
                "/Media/experiences/tour-22/cynthia-hero.jpg?v=1",
                "/Media/experiences/tour-22/cynthia-tasting-07.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-01.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-02.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-03.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-04.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-05.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-06.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-08.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-09.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-10.jpg",
                "/Media/experiences/tour-22/cynthia-tasting-11.jpg"
            };

            await UpsertWorkshopAsync("Tasting Hour", new Experience
            {
                Title = "Peru Tasting Hour",
                TitleES = "Hora de cata del Perú",
                Location = "Cusco",
                Month = "Year-round",
                Difficulty = "All Levels",
                Duration = "1 HOUR",
                Price = 25,
                Tag = "1 HOUR",
                ImageUrl = "/Media/experiences/tour-22/cynthia-hero.jpg?v=1",
                Description = "One hour with Cynthia and Pavel. She leads the sensory side (cupping, roast); he brews V60. Sit, taste, and learn how variety and process — washed, natural, honey — change the cup. All levels; coffee is made for you.",
                DescriptionES = "Una hora con Cynthia y Pavel. Ella lleva lo sensorial (catación, tueste); él prepara V60. Pruebas y aprendes cómo la variedad y el proceso — lavado, natural, honey — cambian la taza. Todos los niveles; el café se hace para ti.",
                LongDescription = "Hosted by Cynthia, roastmaster and cupping specialist, and Pavel, V60 pour-over. You sit; they brew. Cups from around Peru — different varieties and processes — land in front of you while you talk altitude, washed vs natural vs honey, and what those words actually taste like. First specialty cup or already deep in coffee: the conversation meets you there. You leave able to read a bag and know what you like.",
                LongDescriptionES = "Con Cynthia, tostadora y especialista en catación, y Pavel, maestro de V60. Tú te sientas; ellos preparan. Tazas de distintas regiones, variedades y procesos: lavado, natural, honey. Principiante o avanzado: la conversación se adapta. Sales sabiendo leer un empaque y reconocer lo que te gusta.",
                Syllabus = new List<string>
                {
                    "Welcome: Cynthia & Pavel — Peru is not one coffee (10 min)||<ul class=\"tour-syllabus-points\"><li>Cynthia (cupping and roast) and Pavel (V60) open the hour together.</li><li>Peru runs from desert coast to Amazon jungle to peaks above 2,000m — so variety, altitude, and process actually show up in the cup.</li><li>We set that frame before the first sip, whether this is your first specialty coffee or you already know the map.</li></ul>||Bourbon, Geisha, Typica, Altitude, Terroir",
                    "Tasting flight: process & variety side by side (40 min)||<ul class=\"tour-syllabus-points\"><li>Pavel brews; you stay seated.</li><li>Cups from different regions, varieties, and processes land in front of you.</li><li>You learn what makes washed coffee distinct from natural and honey — acidity, fruit, cleanliness, funk — and how long versus short fermentation changes flavor.</li><li>Fragrance on the dry grounds, then aroma once the coffee is wet. Taste side by side: sweetness, acidity, body.</li></ul>||Washed, Natural, Honey, Long fermentation, Short fermentation, Fragrance, Aroma, V60",
                    "Close: your palate, the next bag (10 min)||<ul class=\"tour-syllabus-points\"><li>Pick the cup you liked most.</li><li>Together we name why — sweetness, acidity, body — so you leave knowing what your palate leans toward.</li><li>Next time you pick up a coffee bag, variety and process on the label will point you at coffee you actually like.</li></ul>||Palate, Flavor notes, Reading a bag"
                },
                ProvidedEquipment = new List<string>
                {
                    "Coffees from around Peru, brewed for you — Cynthia and Pavel at the table",
                    "Guided tasting: cupping language with a Q grader, V60 with Pavel",
                    "Water, cups, and a quiet seat — no gear required"
                },
                RequiredGear = new List<string>(),
                GalleryImages = cynthiaTastingGallery
            });

            await UpsertWorkshopAsync("Brew Your Own", new Experience
            {
                Title = "Brew Your Own",
                TitleES = "Prepara tu café",
                Location = "Cusco",
                Month = "Year-round",
                Difficulty = "All Levels",
                Duration = "1 HOUR",
                Price = 25,
                Tag = "1 HOUR",
                ImageUrl = "/Media/experiences/tour-25/cynthia-brew-01.webp?v=2",
                Description = "An hour on the bar — you prepare your own coffee. We explain roast levels and brew ratios, give you a recipe, then you brew it and run it again to sharpen your technique.",
                DescriptionES = "Una hora en la barra: tú preparas tu café. Explicamos tuestes y ratios, te damos una receta, preparas tu taza y afinamos la técnica.",
                LongDescription = "Hands-on hour on the brew bar. We explain how light roast and more developed roast need different water and grind, lock a ratio, and hand you a written recipe. You brew your own cup, we watch the pour and adjust together, then you brew a second time so your technique is better when you leave.",
                LongDescriptionES = "Hora práctica en la barra. Explicamos tueste claro vs desarrollado, agua y molienda, fijamos un ratio y te damos la receta. Preparas tu taza, afinamos juntos y repites para mejorar la técnica.",
                Syllabus = new List<string>
                {
                    "Roast & ratio: light vs developed — water, grind, and your written recipe (15 min)||Why a light roast and a more developed roast need different extraction. We set grind, water temperature, and ratio, then give you a recipe card to follow on the bar.||Light roast, Developed roast, Grind, Ratio, Recipe card",
                    "You brew: prepare your own cup from that recipe (25 min)||You take the brewer, dose, and pour. We coach timing, bloom, and pour pattern while you make the cup yourself — this is the hour where you work the bar, not us.||V60, Bloom, Pour, Dose, Timing",
                    "Technique: second pass — improve pour, timing, and what to change at home (20 min)||You brew again with adjustments. We compare both cups and leave you with clear notes on what to change when you brew at home or in a hotel.||Second brew, Adjustments, Home brew, Technique"
                },
                ProvidedEquipment = new List<string>
                {
                    "Written brew recipe",
                    "V60 or the day’s brewer",
                    "Coffee to brew and to take notes on"
                },
                RequiredGear = new List<string>(),
                GalleryImages = new List<string>
                {
                    "/Media/experiences/tour-25/cynthia-brew-01.webp?v=2",
                    "/Media/experiences/tour-25/cynthia-brew-02.webp?v=2",
                    "/Media/experiences/tour-25/cynthia-brew-03.webp?v=2"
                }
            });

            var cynthiaCuppingGallery = new List<string>
            {
                "/Media/experiences/tour-26/cynthia-cupping-01.webp?v=2",
                "/Media/experiences/tour-26/cynthia-cupping-02.webp?v=2",
                "/Media/experiences/tour-26/cynthia-cupping-03.webp?v=2",
                "/Media/experiences/tour-26/cynthia-cupping-04.webp?v=2",
                "/Media/experiences/tour-26/cynthia-cupping-05.webp?v=2",
                "/Media/experiences/tour-26/cynthia-cupping-06.webp?v=2"
            };

            await UpsertWorkshopAsync("Introduction to Cupping", new Experience
            {
                Title = "Introduction to Cupping",
                TitleES = "Introducción a la catación",
                Location = "Cusco",
                Month = "Year-round",
                Difficulty = "Beginner",
                Duration = "1 HOUR",
                Price = 25,
                Tag = "1 HOUR",
                ImageUrl = "/Media/experiences/tour-26/cynthia-cupping-01.webp?v=2",
                Description = "Four rounds of cupping in one hour. We teach SCA aromatic evaluation — fragrance of the dry grounds, then aroma at the break and wet — then taste four coffees: an 81-point baseline, two specialty lots (~85 and 86–87), and one super lot.",
                DescriptionES = "Cuatro rondas de catación en una hora. Cómo oler — romper la costra, fragrance (molido seco) y aroma al romper la costra — y cuatro cafés: un 81 básico, dos de especialidad (~85 y 86–87) y un súper lote.",
                LongDescription = "Your first cupping, paced for travelers. We open with SCA aromatic protocol: fragrance of the dry grounds, then aroma as you break the crust and as the coffee steeps. Then four coffees in four rounds — an 81-point everyday cup as baseline, two specialty lots around 85 and 86–87, and one super coffee so you feel the jump in quality. Same protocol each round: fragrance, aroma, slurp, note what you taste, talk.",
                LongDescriptionES = "Primera catación a ritmo de viajero. Abrimos con el olfato: romper la costra, fragrance (molido seco) y aroma al romper la costra, activar los sentidos. Luego cuatro cafés: un 81 cotidiano, dos de especialidad (~85 y 86–87) y un súper café. Misma pauta: oler, sorber, anotar, hablar.",
                Syllabus = new List<string>
                {
                    "Fragrance & aroma: dry grounds, break the crust, wet evaluation (10 min)||Before any slurp we teach SCA aromatic evaluation — fragrance of the dry grounds, then aroma as you break the crust and as the coffee steeps. This is the foundation for every round that follows.||Fragrance, Aroma, Break, Dry grounds, Wet",
                    "Round 1: the 81-point cup — everyday coffee as a baseline (12 min)||Your first scored coffee is a solid everyday lot around 81 points. Learn the cupping protocol: smell dry and wet, slurp, and note acidity, body, and sweetness without overthinking it.||81 points, Baseline, Slurp, Acidity, Body",
                    "Rounds 2–3: specialty lots around 85 and 86–87 (25 min)||Two higher-scoring coffees side by side. Notice how clarity, sweetness, and complexity jump compared to the baseline — we talk through what changed on the farm and in the roast.||85 points, 86–87, Clarity, Sweetness, Complexity",
                    "Round 4: the super coffee — what a high score tastes like (13 min)||One exceptional lot to finish. Feel the gap between good, specialty, and truly standout coffee. Close with questions and what to look for when buying beans.||Super lot, High score, Specialty, Buying beans"
                },
                ProvidedEquipment = new List<string>
                {
                    "Four cupping rounds",
                    "Spoons, bowls, and a cupping form",
                    "Four coffees: 81 · ~85 · 86–87 · super lot"
                },
                RequiredGear = new List<string>(),
                GalleryImages = cynthiaCuppingGallery
            });

            await UpsertWorkshopAsync("Cusco Coffee Laboratory", new Experience
            {
                Title = "The Cusco Coffee Laboratory",
                TitleES = "El Laboratorio de Café de Cusco",
                Location = "Cusco",
                Month = "Year-round",
                Difficulty = "All Levels",
                Duration = "2.5hrs",
                Price = 50,
                Tag = "FEATURED",
                ImageUrl = "/Media/experiences/tour-14/lab-hero.jpg",
                Description = "Join us in our state-of-the-art laboratory in the heart of Cusco. Varieties, V60, espresso, and a roast you take home — one session that covers the full craft.",
                DescriptionES = "Únete a nuestro laboratorio en el corazón de Cusco. Variedades, V60, espresso y un tueste para llevar — una sesión que cubre todo el oficio.",
                LongDescription = "Our Cusco Coffee Laboratory workshop elevates your coffee journey. Theory on Peruvian varieties and altitude, V60 filtration and sensory work, an espresso machine demo with cappuccino tasting, then roast fundamentals together — everyone takes home 200g of fresh roast.",
                LongDescriptionES = "Nuestro laboratorio en Cusco eleva tu viaje con el café. Variedades peruanas y altitud, V60 y sensorial, demo de espresso con cappuccino, y fundamentos de tueste — todos se llevan 200g de tueste fresco.",
                Syllabus = new List<string>
                {
                    "Intro: varieties in Peru — brewer, grinder, coffee, and ratio are set; every cup comes out different (15 min)||Explanation of varieties growing across Peru and why altitude matters in Cusco. Everything is prepared for you: brewer, grinder setting, coffee dose, and a basic recipe. All brews will taste different — that is the point of the session.||Varieties, Altitude, Ratio, Brewer, Grinder",
                    "V60 Filtration & Sensory: intro to the V60, grinding, and sensory evaluation (30 min)||We introduce the V60 and grinding, then evaluate aroma (compare to other drinks you know), taste notes using spoons, and mouthfeel — especially acidity. You learn to name what you sense before moving to espresso.||V60, Grinding, Aroma, Acidity, Sensory",
                    "Espresso Experience: machine demo — grind, weigh, tamp, extract, and cappuccino tasting (30 min)||Introduction to the espresso machine. We demonstrate grinding, weighing, tamping, and ratio extraction, froth milk for a cappuccino, and taste and evaluate the result together.||Espresso, Tamp, Extraction, Cappuccino, Milk",
                    "Roasting Experience: roast fundamentals together — everyone takes home 200g of fresh roast (1 hr)||We run a batch roast together and cover the fundamentals of the roasting process. At the end, everyone takes home 200 grams of freshly roasted coffee.||Roast, Batch, Fresh roast, Take-home, 200g"
                },
                ProvidedEquipment = new List<string>
                {
                    "V60 and espresso gear",
                    "Guided sensory tasting",
                    "Batch roast — 200g to take home"
                },
                RequiredGear = new List<string>(),
                GalleryImages = new List<string>()
            });

            await UpsertWorkshopAsync("Odar", new Experience
            {
                Title = "Odar Lab [Sensory & Roast]",
                TitleES = "Lab Odar [Sensorial y tueste]",
                Location = "Cusco",
                Month = "Year-round",
                Difficulty = "All Levels",
                Duration = "2.5 HOURS",
                Price = 50,
                ImageUrl = odarGallery[0],
                Description = "Hosted by Odar, a certified Q grader, at his factory and café two minutes apart in Cusco. Same craft as the main lab, with more time on sensory: Peruvian varieties, V60 at the factory, a guided SCA cupping, then roast-floor work — sorting greens, spotting quakers, and tasting acidity in the bean.",
                DescriptionES = "Con Odar, Q grader certificado, en su fábrica y café a dos minutos en Cusco. Más tiempo en lo sensorial: variedades del Perú, V60 en fábrica, catación SCA guiada y trabajo en tueste.",
                LongDescription = "Odar owns a factory and a café a two-minute walk apart in Cusco. He covers the same ground as the featured laboratory — brew, sensory, roast — with the emphasis on tasting. Ten minutes on varieties in Peru and Cusco and why altitude matters, then a V60 at the factory: how the cone works, ratio, and a cup. After that, the flavor wheel, what the SCA is, how a cupping form is scored, and a guided cupping with Odar. Close on the roast floor: sort green beans before they go in, pick quakers and defects after, and bite a bean to feel acidity.",
                LongDescriptionES = "Odar tiene fábrica y café a dos minutos en Cusco. Diez minutos de variedades y altitud, V60 en la fábrica, rueda de sabores y catación guiada, y al final clasificación de verde, quakers y morder el grano para la acidez.",
                Syllabus = new List<string>
                {
                    "Intro & V60: Peru and Cusco varieties, high altitude (10 min), then how the cone works, ratio, and a brew at the factory",
                    "Sensory: flavor wheel, the SCA, cupping forms, then a guided cupping with Odar (Q grader)",
                    "Roast: sort greens with the roast master, pick quakers and defects, bite the bean for acidity"
                },
                ProvidedEquipment = new List<string>
                {
                    "V60 brew at the factory",
                    "Guided SCA cupping with a Q grader",
                    "Green-bean sorting and roast-floor tasting",
                    "Apron and cupping spoons"
                },
                RequiredGear = new List<string> { "Closed-toe shoes" },
                GalleryImages = odarGallery
            });

            await UpsertWorkshopAsync("Cinthya", new Experience
            {
                Title = "Cinthya Lab [V60, Espresso & Cupping]",
                TitleES = "Lab Cinthya [V60, espresso y catación]",
                Location = "Cusco",
                Month = "Year-round",
                Difficulty = "All Levels",
                Duration = "2.5 HOURS",
                Price = 50,
                ImageUrl = "",
                Description = "Hosted by Cinthya, a Q grader, competition judge, and roaster. Her roaster sits away from the tasting room, so this session stays on the bar: V60 pour-over, espresso, and a guided cupping. A strong option when the main laboratory is full.",
                DescriptionES = "Con Cinthya, Q grader, jueza y tostadora. El tostador queda lejos del área de cata, así que la sesión es V60, espresso y catación guiada. Alternativa cuando el laboratorio principal está lleno.",
                LongDescription = "Cinthya is a roaster, a Q grader, and a judge. Her roaster is not next to the presentation space, so this tour does not walk the roast floor. You stay with brew and sensory: V60 pour-over, espresso on the machine, and a cupping she leads. Photos of her space will go up when we have them.",
                LongDescriptionES = "Cinthya es tostadora, Q grader y jueza. El tostador no está junto al espacio de presentación, así que el recorrido es V60, espresso y catación. Subiremos fotos de su local cuando las tengamos.",
                Syllabus = new List<string>
                {
                    "V60 pour-over: grind, ratio, and a cup you brew",
                    "Espresso: extraction, tasting, and how the bar works",
                    "Guided cupping: SCA palate work with a Q grader and judge"
                },
                ProvidedEquipment = new List<string>
                {
                    "V60 pour-over",
                    "Espresso tasting",
                    "Guided cupping"
                },
                RequiredGear = new List<string>(),
                GalleryImages = new List<string>()
            });

            async Task UpsertTourSiteContentAsync(string key, string value)
            {
                var row = await _context.SiteContent.FirstOrDefaultAsync(c => c.Key == key);
                if (row == null)
                {
                    _context.SiteContent.Add(new SiteContent { Key = key, Value = value, Page = "Tour" });
                }
                else
                {
                    row.Value = value;
                    row.Page = "Tour";
                }
            }

            await UpsertTourSiteContentAsync("Tour_Hero_Image_26", "/Media/experiences/tour-26/cynthia-cupping-06.webp?v=2");
            await UpsertTourSiteContentAsync("Tour_Hero_Pos_26", "50% 40%");

            // Cinthya Lab (2.5hr) archived — the three 1-hour workshops above are the distinct offerings.
            var cinthyaLab = await _context.Experiences.FirstOrDefaultAsync(e => e.Title == "Cinthya Lab [V60, Espresso & Cupping]");
            if (cinthyaLab != null)
            {
                cinthyaLab.Category = "Archived";
            }

            var keep = new[]
            {
                "Peru Tasting Hour",
                "Brew Your Own",
                "Introduction to Cupping",
                "The Cusco Coffee Laboratory",
                "Odar Lab [Sensory & Roast]"
            };
            var extras = await _context.Experiences
                .Where(e => (e.Category == "Urban Workshops" || e.Category == "Urban Labs") && !keep.Contains(e.Title))
                .ToListAsync();
            foreach (var extra in extras)
            {
                extra.Category = "Archived";
            }

            var labsSubtitle = await _context.SiteContent.FirstOrDefaultAsync(c => c.Key == "Experiences_UrbanLabs_Subtitle");
            if (labsSubtitle != null)
            {
                labsSubtitle.Value = "CUSCO";
            }
            else
            {
                _context.SiteContent.Add(new SiteContent
                {
                    Key = "Experiences_UrbanLabs_Subtitle",
                    Value = "CUSCO",
                    Page = "Experiences"
                });
            }
        }
    }
}
