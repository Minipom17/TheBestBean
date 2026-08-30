using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBestBean.Migrations
{
    /// <inheritdoc />
    public partial class MakeProductFullyEditable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MapImageUrl",
                table: "CoffeeBean",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginDescription",
                table: "CoffeeBean",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProcessingDescription",
                table: "CoffeeBean",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProducerDescription",
                table: "CoffeeBean",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProducerImageUrl",
                table: "CoffeeBean",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapImageUrl",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "OriginDescription",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "ProcessingDescription",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "ProducerDescription",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "ProducerImageUrl",
                table: "CoffeeBean");
        }
    }
}
