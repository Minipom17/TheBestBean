using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBestBean.Migrations
{
    /// <inheritdoc />
    public partial class ExpandExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Experiences",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GalleryImages",
                table: "Experiences",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LongDescription",
                table: "Experiences",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProvidedEquipment",
                table: "Experiences",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequiredGear",
                table: "Experiences",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Syllabus",
                table: "Experiences",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "GalleryImages",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "LongDescription",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "ProvidedEquipment",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "RequiredGear",
                table: "Experiences");

            migrationBuilder.DropColumn(
                name: "Syllabus",
                table: "Experiences");
        }
    }
}
