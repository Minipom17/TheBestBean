using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TheBestBean.Data;

#nullable disable

namespace TheBestBean.Migrations
{
    [DbContext(typeof(TheBestBeanContext))]
    [Migration("20260916203000_AddOrderMercadoPago")]
    public partial class AddOrderMercadoPago : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "Pending");

            migrationBuilder.AddColumn<string>(
                name: "MercadoPagoPreferenceId",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MercadoPagoPaymentId",
                table: "Orders",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "PaymentStatus", table: "Orders");
            migrationBuilder.DropColumn(name: "MercadoPagoPreferenceId", table: "Orders");
            migrationBuilder.DropColumn(name: "MercadoPagoPaymentId", table: "Orders");
        }
    }
}
