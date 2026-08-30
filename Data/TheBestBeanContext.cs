using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheBestBean.Models;
using System.Text.Json;

namespace TheBestBean.Data // Updated for consistency
{
        // The class that handles the database connection and defines the tables
        public class TheBestBeanContext : IdentityDbContext<IdentityUser>
    {
        public TheBestBeanContext(DbContextOptions<TheBestBeanContext> options)
            : base(options)
        {
        }

        // Define the tables (DbSets) for our application
        public DbSet<CoffeeBean> CoffeeBean { get; set; } = default!;
        public DbSet<CoffeeFarm> CoffeeFarm { get; set; } = default!;
        public DbSet<CoffeeRegion> CoffeeRegion { get; set; } = default!;
        public DbSet<OriginCountry> OriginCountry { get; set; } = default!;
        public DbSet<FarmerSurvey> FarmerSurveys { get; set; } = default!;
        public DbSet<FarmProfile> FarmProfiles { get; set; } = default!;
        public DbSet<BlogPost> BlogPosts { get; set; } = default!;

        // RoastOS tables
        public DbSet<BeanInventory> BeanInventories { get; set; } = default!;
        public DbSet<RoastBatch> RoastBatches { get; set; } = default!;
        public DbSet<CuppingScore> CuppingScores { get; set; } = default!;

        // Experiences
        public DbSet<Experience> Experiences { get; set; } = default!;

        // CMS Content
        public DbSet<SiteContent> SiteContent { get; set; } = default!;
        public DbSet<FlavorZone> FlavorZones { get; set; } = default!;

        // Orders
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderItem> OrderItems { get; set; } = default!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call base OnModelCreating
            base.OnModelCreating(modelBuilder);

            // Seed data will be added after database creation
            // SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Only seed if FarmerSurveys and FarmProfiles are empty
            modelBuilder.Entity<FarmProfile>().HasData(
                new FarmProfile
                {
                    Id = 1,
                    FarmName = "Finca El Paraiso",
                    FarmerName = "María Elena Rodríguez",
                    Region = "Cajamarca, Perú",
                    GPSLatitude = "-7.1631",
                    GPSLongitude = "-78.5303",
                    FarmPhotos = JsonSerializer.Serialize(new[] { "/images/farm1.jpg", "/images/farm2.jpg", "/images/farm3.jpg" }),
                    FarmStory = "Nestled in the highlands of Cajamarca, Finca El Paraiso has been producing exceptional coffee for over three generations. María Elena carries forward her family's traditions while embracing sustainable farming practices.",
                    CoffeeVarieties = "Typica, Bourbon, Caturra",
                    ProcessingMethod = "Washed and Honey",
                    FarmingPhilosophy = "Sustainable farming that respects the land and supports our community while producing the highest quality coffee.",
                    EnvironmentalFeatures = "Diverse shade canopy with native trees, natural water springs, rich volcanic soil, and extensive biodiversity corridors.",
                    ClimateNotes = "High altitude (1,750m) creates cooler temperatures and slower cherry development, resulting in concentrated flavors.",
                    QualityHighlights = "Complex acidity, floral notes, and exceptional sweetness that reflects our mountain terroir.",
                    QualityRating = 8,
                    SustainabilityPractices = "Organic fertilizer production from farm waste, water conservation, and soil regeneration techniques.",
                    CommunityImpact = "Provides stable employment to local families and supports community education programs.",
                    CreatedDate = new DateTime(2024, 1, 1),
                    LastUpdated = new DateTime(2024, 1, 26),
                    IsActive = true
                },
                new FarmProfile
                {
                    Id = 2,
                    FarmName = "Los Pinos Specialty Coffee",
                    FarmerName = "Carlos Andrés Quintero",
                    Region = "Huila, Colombia",
                    GPSLatitude = "2.9309",
                    GPSLongitude = "-75.2815",
                    FarmPhotos = JsonSerializer.Serialize(new[] { "/images/colombia1.jpg", "/images/colombia2.jpg" }),
                    FarmStory = "In the coffee heartland of Huila, Carlos continues his family's legacy of specialty coffee production. The farm's unique microclimate creates distinctive flavors prized by coffee connoisseurs worldwide.",
                    CoffeeVarieties = "Castillo, Caturra, Bourbon",
                    ProcessingMethod = "Fully Washed",
                    FarmingPhilosophy = "Focus on quality over quantity, maintaining traditional methods while implementing modern quality control.",
                    EnvironmentalFeatures = "Mountain stream irrigation, fertile volcanic soil, and a mix of native and fruit trees providing natural shade.",
                    ClimateNotes = "Consistent rainfall patterns and moderate temperatures create ideal conditions for Arabica cultivation.",
                    QualityHighlights = "Bright acidity, citrus notes, and clean finish typical of the Huila region.",
                    QualityRating = 9,
                    SustainabilityPractices = "Rainwater harvesting, organic pest management, and sustainable soil practices.",
                    CommunityImpact = "Supports local cooperative and contributes to infrastructure development in surrounding communities.",
                    CreatedDate = new DateTime(2024, 1, 15),
                    LastUpdated = new DateTime(2024, 1, 28),
                    IsActive = true
                }
            );
        }
    }
}


