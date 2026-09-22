using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TheBestBean.Data;

#nullable disable

namespace TheBestBean.Migrations
{
    [DbContext(typeof(TheBestBeanContext))]
    [Migration("20260922010000_AddGreenLotTrailDates")]
    public partial class AddGreenLotTrailDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "HarvestedOn",
                table: "BeanInventories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FermentedOn",
                table: "BeanInventories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DriedOn",
                table: "BeanInventories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivedCuscoOn",
                table: "BeanInventories",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "HarvestedOn", table: "BeanInventories");
            migrationBuilder.DropColumn(name: "FermentedOn", table: "BeanInventories");
            migrationBuilder.DropColumn(name: "DriedOn", table: "BeanInventories");
            migrationBuilder.DropColumn(name: "ArrivedCuscoOn", table: "BeanInventories");
        }
    }
}
