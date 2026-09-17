using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TheBestBean.Data;

#nullable disable

namespace TheBestBean.Migrations
{
    [DbContext(typeof(TheBestBeanContext))]
    [Migration("20260917153000_AddBeanInventoryCoffeeBeanId")]
    public partial class AddBeanInventoryCoffeeBeanId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoffeeBeanId",
                table: "BeanInventories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BeanInventories_CoffeeBeanId",
                table: "BeanInventories",
                column: "CoffeeBeanId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BeanInventories_CoffeeBeanId",
                table: "BeanInventories");

            migrationBuilder.DropColumn(
                name: "CoffeeBeanId",
                table: "BeanInventories");
        }
    }
}
