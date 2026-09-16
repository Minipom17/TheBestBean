using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TheBestBean.Data;

#nullable disable

namespace TheBestBean.Migrations
{
    [DbContext(typeof(TheBestBeanContext))]
    [Migration("20260916204500_AddOrderPayPal")]
    public partial class AddOrderPayPal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PayPalOrderId",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PayPalOrderId", table: "Orders");
        }
    }
}
