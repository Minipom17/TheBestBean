using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBestBean.Migrations
{
    /// <inheritdoc />
    public partial class AddGreenStockImageAndProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FlavorProfile",
                table: "BeanInventories",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "BeanInventories",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ScaScore",
                table: "BeanInventories",
                type: "decimal(5, 2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlavorProfile",
                table: "BeanInventories");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "BeanInventories");

            migrationBuilder.DropColumn(
                name: "ScaScore",
                table: "BeanInventories");
        }
    }
}
