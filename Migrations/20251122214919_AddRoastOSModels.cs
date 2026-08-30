using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBestBean.Migrations
{
    /// <inheritdoc />
    public partial class AddRoastOSModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeanInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Farmer = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    Process = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TotalKg = table.Column<decimal>(type: "decimal(10, 2)", nullable: false),
                    CostPerKg = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    PurchaseDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LotNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Region = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Variety = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ElevationMeters = table.Column<int>(type: "INTEGER", nullable: true),
                    HarvestYear = table.Column<int>(type: "INTEGER", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeanInventories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoastBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BeanInventoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoastDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RoastTimeSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    GreenWeightGrams = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    RoastedWeightGrams = table.Column<decimal>(type: "decimal(10, 2)", nullable: true),
                    WeightLossPercent = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    DTRPercent = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    FirstCrackTime = table.Column<int>(type: "INTEGER", nullable: true),
                    DropTime = table.Column<int>(type: "INTEGER", nullable: true),
                    TurnPointTime = table.Column<int>(type: "INTEGER", nullable: true),
                    DryEndTime = table.Column<int>(type: "INTEGER", nullable: true),
                    TemperatureDataJson = table.Column<string>(type: "TEXT", nullable: true),
                    EventsJson = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    RoastLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoastBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoastBatches_BeanInventories_BeanInventoryId",
                        column: x => x.BeanInventoryId,
                        principalTable: "BeanInventories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuppingScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoastBatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    CupperName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    CuppingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LotNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    RoastLevel = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    FragranceDry = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    FragranceBreak = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    FragranceScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    FlavorScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    AftertasteScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    AcidityScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    AcidityIntensity = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    BodyScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    BodyIntensity = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    BalanceScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    UniformityChecks = table.Column<int>(type: "INTEGER", nullable: true),
                    SweetnessChecks = table.Column<int>(type: "INTEGER", nullable: true),
                    CleanCupChecks = table.Column<int>(type: "INTEGER", nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    DefectCups = table.Column<int>(type: "INTEGER", nullable: true),
                    FinalScore = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuppingScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuppingScores_RoastBatches_RoastBatchId",
                        column: x => x.RoastBatchId,
                        principalTable: "RoastBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuppingScores_RoastBatchId",
                table: "CuppingScores",
                column: "RoastBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_RoastBatches_BeanInventoryId",
                table: "RoastBatches",
                column: "BeanInventoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CuppingScores");

            migrationBuilder.DropTable(
                name: "RoastBatches");

            migrationBuilder.DropTable(
                name: "BeanInventories");
        }
    }
}
