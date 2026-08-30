using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBestBean.Migrations
{
    /// <inheritdoc />
    public partial class AddDrinkEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DrinkEvaluations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoastBatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    DrinkType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BaristaName = table.Column<string>(type: "TEXT", maxLength: 150, nullable: true),
                    EvaluationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CremaColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    CremaThickness = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    CremaPersistence = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    AromaIntensity = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    AromaQuality = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    AromaNotes = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BalanceScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    Sweetness = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    Acidity = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    Bitterness = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    BodyScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    Mouthfeel = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    PrimaryFlavors = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    SecondaryFlavors = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    AftertasteQuality = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    AftertasteLength = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    ExtractionTimeSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    DoseGrams = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    YieldGrams = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    Ratio = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    TemperatureCelsius = table.Column<decimal>(type: "decimal(5, 2)", nullable: true),
                    MilkIntegrationScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    MilkNotes = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    OverallScore = table.Column<decimal>(type: "decimal(4, 2)", nullable: true),
                    Recommended = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrinkEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrinkEvaluations_RoastBatches_RoastBatchId",
                        column: x => x.RoastBatchId,
                        principalTable: "RoastBatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrinkEvaluations_RoastBatchId",
                table: "DrinkEvaluations",
                column: "RoastBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrinkEvaluations");
        }
    }
}
