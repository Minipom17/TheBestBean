using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBestBean.Migrations
{
    /// <inheritdoc />
    public partial class EnhancedProductDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Altitude",
                table: "CoffeeBean",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "CoffeeBean",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Producer",
                table: "CoffeeBean",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Variety",
                table: "CoffeeBean",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altitude",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "Producer",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "Variety",
                table: "CoffeeBean");
        }
    }
}
