using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBestBean.Migrations
{
    /// <inheritdoc />
    public partial class InitialCoffeeSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoffeeFarm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeFarm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoffeeRegion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeRegion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FarmerSurveys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FarmName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    FarmerName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Region = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    GpsCoordinates = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SurveyDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ElevationMeters = table.Column<int>(type: "INTEGER", nullable: true),
                    SlopeType = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Aspect = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TemperatureRange = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RainfallPatterns = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    WaterSource = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TotalFarmSizeHa = table.Column<decimal>(type: "TEXT", nullable: true),
                    CoffeePlantedAreaHa = table.Column<decimal>(type: "TEXT", nullable: true),
                    VarietiesGrown = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    PlantingDensityTreesHa = table.Column<int>(type: "INTEGER", nullable: true),
                    ShadeTrees = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    OtherCrops = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    HasIntercropping = table.Column<bool>(type: "INTEGER", nullable: false),
                    IntercroppingDetails = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SoilType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SoilDepth = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DrainageConditions = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ErosionRisks = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SoilFertilityManagement = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CoffeePlantingYear = table.Column<int>(type: "INTEGER", nullable: true),
                    PruningMethod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FertilizationType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PestDiseasePresence = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    HarvestingMethod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ProcessingMethod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    FermentationType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DryingMethod = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DryingTime = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StorageConditions = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TraceabilitySystem = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LaborSystem = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CooperativeMembership = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Certifications = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AnnualProductionKg = table.Column<decimal>(type: "TEXT", nullable: true),
                    UniqueQualities = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ChallengesMentioned = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    PhotographsTaken = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    SurveyNotes = table.Column<string>(type: "TEXT", nullable: false),
                    FollowUpRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    QualityRating = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmerSurveys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OriginCountry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OriginCountry", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FarmProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FarmName = table.Column<string>(type: "TEXT", nullable: false),
                    FarmerName = table.Column<string>(type: "TEXT", nullable: false),
                    Region = table.Column<string>(type: "TEXT", nullable: false),
                    GPSLatitude = table.Column<string>(type: "TEXT", nullable: true),
                    GPSLongitude = table.Column<string>(type: "TEXT", nullable: true),
                    RelatedSurveyId = table.Column<int>(type: "INTEGER", nullable: true),
                    FarmPhotos = table.Column<string>(type: "TEXT", nullable: false),
                    FarmStory = table.Column<string>(type: "TEXT", nullable: false),
                    CoffeeVarieties = table.Column<string>(type: "TEXT", nullable: false),
                    ProcessingMethod = table.Column<string>(type: "TEXT", nullable: false),
                    FarmingPhilosophy = table.Column<string>(type: "TEXT", nullable: false),
                    EnvironmentalFeatures = table.Column<string>(type: "TEXT", nullable: false),
                    ClimateNotes = table.Column<string>(type: "TEXT", nullable: false),
                    QualityHighlights = table.Column<string>(type: "TEXT", nullable: false),
                    QualityRating = table.Column<int>(type: "INTEGER", nullable: true),
                    SustainabilityPractices = table.Column<string>(type: "TEXT", nullable: false),
                    CommunityImpact = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FarmProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FarmProfiles_FarmerSurveys_RelatedSurveyId",
                        column: x => x.RelatedSurveyId,
                        principalTable: "FarmerSurveys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CoffeeBean",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    FlavorProfile = table.Column<string>(type: "TEXT", maxLength: 250, nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3, 1)", nullable: false),
                    ProcessingMethod = table.Column<string>(type: "TEXT", nullable: false),
                    CoffeeFarmId = table.Column<int>(type: "INTEGER", nullable: false),
                    CoffeeRegionId = table.Column<int>(type: "INTEGER", nullable: false),
                    OriginCountryId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoffeeBean", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoffeeBean_CoffeeFarm_CoffeeFarmId",
                        column: x => x.CoffeeFarmId,
                        principalTable: "CoffeeFarm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoffeeBean_CoffeeRegion_CoffeeRegionId",
                        column: x => x.CoffeeRegionId,
                        principalTable: "CoffeeRegion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoffeeBean_OriginCountry_OriginCountryId",
                        column: x => x.OriginCountryId,
                        principalTable: "OriginCountry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeBean_CoffeeFarmId",
                table: "CoffeeBean",
                column: "CoffeeFarmId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeBean_CoffeeRegionId",
                table: "CoffeeBean",
                column: "CoffeeRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeBean_OriginCountryId",
                table: "CoffeeBean",
                column: "OriginCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_FarmProfiles_RelatedSurveyId",
                table: "FarmProfiles",
                column: "RelatedSurveyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoffeeBean");

            migrationBuilder.DropTable(
                name: "FarmProfiles");

            migrationBuilder.DropTable(
                name: "CoffeeFarm");

            migrationBuilder.DropTable(
                name: "CoffeeRegion");

            migrationBuilder.DropTable(
                name: "OriginCountry");

            migrationBuilder.DropTable(
                name: "FarmerSurveys");
        }
    }
}
