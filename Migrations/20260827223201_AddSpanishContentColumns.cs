using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBestBean.Migrations
{
    /// <inheritdoc />
    public partial class AddSpanishContentColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionES",
                table: "Experiences",
                type: "TEXT",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LongDescriptionES",
                table: "Experiences",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleES",
                table: "Experiences",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FlavorProfileES",
                table: "CoffeeBean",
                type: "TEXT",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OriginDescriptionES",
                table: "CoffeeBean",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProcessingDescriptionES",
                table: "CoffeeBean",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProducerDescriptionES",
                table: "CoffeeBean",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentES",
                table: "BlogPosts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TitleES",
                table: "BlogPosts",
                type: "TEXT",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionES",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "LongDescriptionES",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "TitleES",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "FlavorProfileES",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "OriginDescriptionES",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "ProcessingDescriptionES",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "ProducerDescriptionES",
                table: "CoffeeBean");

            migrationBuilder.DropColumn(
                name: "ContentES",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "TitleES",
                table: "BlogPosts");
        }
    }
}
