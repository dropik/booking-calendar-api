using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingCalendarApi.NETFramework.Migrations
{
    public partial class AddStructureSecrets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ASPassword",
                table: "Structures",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ASUtente",
                table: "Structures",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ASWsKey",
                table: "Structures",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "C59Password",
                table: "Structures",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "C59Struttura",
                table: "Structures",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "C59Username",
                table: "Structures",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IperbookingHotel",
                table: "Structures",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IperbookingPassword",
                table: "Structures",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IperbookingUsername",
                table: "Structures",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Structures",
                keyColumn: "Id",
                keyValue: -1L,
                columns: new[] { "ASPassword", "ASUtente", "ASWsKey", "C59Password", "C59Struttura", "C59Username", "IperbookingHotel", "IperbookingPassword", "IperbookingUsername" },
                values: new object[] { "", "", "", "", "", "", "", "", "" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ASPassword",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "ASUtente",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "ASWsKey",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "C59Password",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "C59Struttura",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "C59Username",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "IperbookingHotel",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "IperbookingPassword",
                table: "Structures");

            migrationBuilder.DropColumn(
                name: "IperbookingUsername",
                table: "Structures");
        }
    }
}
