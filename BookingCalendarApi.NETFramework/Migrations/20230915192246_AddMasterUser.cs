using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Configuration;

namespace BookingCalendarApi.NETFramework.Migrations
{
    public partial class AddMasterUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Structures",
                columns: new[] { "Id", "Name" },
                values: new object[] { -1L, "Master Hotel" });

            var password = ConfigurationManager.AppSettings["MasterPassword"];
            var passwordHasher = new PasswordHasher<string>();
            var passwordHash = passwordHasher.HashPassword("master@bookingcalendar.com", password);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "IsAdmin", "PasswordHash", "StructureId", "Username", "VisibleName" },
                values: new object[] { -1L, true, passwordHash, -1L, "master@bookingcalendar.com", "Master" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1L);

            migrationBuilder.DeleteData(
                table: "Structures",
                keyColumn: "Id",
                keyValue: -1L);

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Users");
        }
    }
}
