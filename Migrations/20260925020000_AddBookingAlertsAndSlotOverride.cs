using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TheBestBean.Data;

#nullable disable

namespace TheBestBean.Migrations
{
    [DbContext(typeof(TheBestBeanContext))]
    [Migration("20260925020000_AddBookingAlertsAndSlotOverride")]
    public partial class AddBookingAlertsAndSlotOverride : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BypassCutoff",
                table: "ExperienceSlots",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DayBeforeAlertSent",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MorningOfAlertSent",
                table: "OrderItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "BypassCutoff", table: "ExperienceSlots");
            migrationBuilder.DropColumn(name: "DayBeforeAlertSent", table: "OrderItems");
            migrationBuilder.DropColumn(name: "MorningOfAlertSent", table: "OrderItems");
        }
    }
}
