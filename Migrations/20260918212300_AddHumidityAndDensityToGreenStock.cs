using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBestBean.Migrations
{
    /// <inheritdoc />
    public partial class AddHumidityAndDensityToGreenStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Density",
                table: "BeanInventories",
                type: "decimal(10, 2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Humidity",
                table: "BeanInventories",
                type: "decimal(5, 2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Density",
                table: "BeanInventories");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "BeanInventories");
        }
    }
}
