using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Data;
using TheBestBean.Models;

namespace TheBestBean.Pages.CoffeeBeans
{
    public class IndexModel : PageModel
    {
        private readonly TheBestBeanContext _context;

        public IndexModel(TheBestBeanContext context)
        {
            _context = context;
        }

        public IList<CoffeeBean> CoffeeBean { get; set; } = default!;
        public Dictionary<string, int?> FarmProfileIds { get; set; } = new Dictionary<string, int?>();

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OriginCountry { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CoffeeRegion { get; set; }

        public async Task OnGetAsync()
        {
            if (_context.CoffeeBean != null)
            {
                var coffeeBeans = from c in _context.CoffeeBean.Include(c => c.CoffeeFarm).Include(c => c.CoffeeRegion).Include(c => c.OriginCountry)
                                  select c;

                if (!string.IsNullOrEmpty(SearchString))
                {
                    coffeeBeans = coffeeBeans.Where(s => s.Name!.Contains(SearchString) ||
                                                        s.ProcessingMethod!.Contains(SearchString) ||
                                                        s.FlavorProfile!.Contains(SearchString) ||
                                                        s.CoffeeFarm.Name.Contains(SearchString) ||
                                                        s.OriginCountry.Name.Contains(SearchString) ||
                                                        s.CoffeeRegion.Name.Contains(SearchString));
                }

                if (!string.IsNullOrEmpty(OriginCountry))
                {
                    coffeeBeans = coffeeBeans.Where(s => s.OriginCountry.Name == OriginCountry);
                }

                if (!string.IsNullOrEmpty(CoffeeRegion))
                {
                    coffeeBeans = coffeeBeans.Where(s => s.CoffeeRegion.Name == CoffeeRegion);
                }

                CoffeeBean = await coffeeBeans.ToListAsync();

                // Load farm profile IDs for each farm
                foreach (var bean in CoffeeBean)
                {
                    if (!FarmProfileIds.ContainsKey(bean.CoffeeFarm.Name))
                    {
                        var farmProfile = await _context.FarmProfiles
                            .FirstOrDefaultAsync(fp => fp.FarmName.ToLower() == bean.CoffeeFarm.Name.ToLower() && fp.IsActive);
                        FarmProfileIds[bean.CoffeeFarm.Name] = farmProfile?.Id;
                    }
                }
            }
        }

        public async Task<IActionResult> OnGetGeoDataAsync(string? level, string? country)
        {
            if (string.IsNullOrWhiteSpace(level) || string.IsNullOrWhiteSpace(country))
            {
                return BadRequest(new { error = "level and country are required" });
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", $"{country.ToLower()}_{level.ToLower()}.geojson");
            if (System.IO.File.Exists(filePath))
            {
                var json = await System.IO.File.ReadAllTextAsync(filePath);
                return Content(json, "application/json");
            }

            if (!country.Equals("PER", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound(new { error = $"No GeoJSON found for {country}" });
            }

            var fallbackJson = level.Equals("adm1", StringComparison.OrdinalIgnoreCase)
                ? GetMinimalPeruAdm1()
                : GetMinimalPeruAdm2();

            return Content(fallbackJson, "application/json");
        }

        private string GetMinimalPeruAdm1()
        {
            // Minimal valid GeoJSON for Peru ADM1 regions (coffee-producing areas)
            return @"{
                ""type"": ""FeatureCollection"",
                ""features"": [
                    {
                        ""type"": ""Feature"",
                        ""properties"": { ""shapeName"": ""Cusco"", ""NAME_1"": ""Cusco"" },
                        ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[-74.5, -14.5], [-73.0, -14.5], [-73.0, -12.5], [-74.5, -12.5], [-74.5, -14.5]]] }
                    },
                    {
                        ""type"": ""Feature"",
                        ""properties"": { ""shapeName"": ""Junín"", ""NAME_1"": ""Junin"" },
                        ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[-76.0, -12.0], [-74.5, -12.0], [-74.5, -10.5], [-76.0, -10.5], [-76.0, -12.0]]] }
                    },
                    {
                        ""type"": ""Feature"",
                        ""properties"": { ""shapeName"": ""San Martín"", ""NAME_1"": ""San Martin"" },
                        ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[-77.0, -7.0], [-75.5, -7.0], [-75.5, -5.5], [-77.0, -5.5], [-77.0, -7.0]]] }
                    },
                    {
                        ""type"": ""Feature"",
                        ""properties"": { ""shapeName"": ""Cajamarca"", ""NAME_1"": ""Cajamarca"" },
                        ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[-79.0, -7.5], [-77.5, -7.5], [-77.5, -6.0], [-79.0, -6.0], [-79.0, -7.5]]] }
                    },
                    {
                        ""type"": ""Feature"",
                        ""properties"": { ""shapeName"": ""Pasco"", ""NAME_1"": ""Pasco"" },
                        ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[-76.5, -11.0], [-75.0, -11.0], [-75.0, -9.5], [-76.5, -9.5], [-76.5, -11.0]]] }
                    },
                    {
                        ""type"": ""Feature"",
                        ""properties"": { ""shapeName"": ""Amazonas"", ""NAME_1"": ""Amazonas"" },
                        ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[-78.5, -6.5], [-77.0, -6.5], [-77.0, -5.0], [-78.5, -5.0], [-78.5, -6.5]]] }
                    },
                    {
                        ""type"": ""Feature"",
                        ""properties"": { ""shapeName"": ""Puno"", ""NAME_1"": ""Puno"" },
                        ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[-70.5, -16.5], [-69.0, -16.5], [-69.0, -15.0], [-70.5, -15.0], [-70.5, -16.5]]] }
                    }
                ]
            }";
        }

        private string GetMinimalPeruAdm2()
        {
            // Minimal valid GeoJSON for Cusco ADM2 (provinces), highlighting La Convención
            return @"{
                ""type"": ""FeatureCollection"",
                ""features"": [
                    {
                        ""type"": ""Feature"",
                        ""properties"": { ""NAME_1"": ""Cusco"", ""shapeName_1"": ""Cusco"", ""NAME_2"": ""La Convención"", ""shapeName_2"": ""La Convencion"" },
                        ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[-73.5, -13.0], [-72.5, -13.0], [-72.5, -12.0], [-73.5, -12.0], [-73.5, -13.0]]] }
                    },
                    {
                        ""type"": ""Feature"",
                        ""properties"": { ""NAME_1"": ""Cusco"", ""shapeName_1"": ""Cusco"", ""NAME_2"": ""Cusco"", ""shapeName_2"": ""Cusco"" },
                        ""geometry"": { ""type"": ""Polygon"", ""coordinates"": [[[-72.2, -13.6], [-71.8, -13.6], [-71.8, -13.2], [-72.2, -13.2], [-72.2, -13.6]]] }
                    }
                ]
            }";
        }
    }
}
