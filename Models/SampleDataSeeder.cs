using Microsoft.AspNetCore.Identity;
using TheBestBean.Data;
using Microsoft.EntityFrameworkCore;

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
            // Let's just save changes now to be safe.
            await _context.SaveChangesAsync();
        }
    }
}
