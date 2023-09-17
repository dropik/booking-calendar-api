using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingCalendarApi.NETFramework.Migrations
{
    public partial class C59StrutturaLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Structures",
                keyColumn: "Id",
                keyValue: -1L,
                column: "C59Struttura",
                value: "0");

            migrationBuilder.AlterColumn<long>(
                name: "C59Struttura",
                table: "Structures",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "varchar(16) CHARACTER SET utf8mb4",
                oldMaxLength: 16,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "C59Struttura",
                table: "Structures",
                type: "varchar(16) CHARACTER SET utf8mb4",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(long),
                oldDefaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "Structures",
                keyColumn: "Id",
                keyValue: -1L,
                column: "C59Struttura",
                value: "");
        }
    }
}
