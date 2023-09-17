using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Repository.NETCore.Migrations
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
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "varchar(16)",
                oldMaxLength: 16,
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "C59Struttura",
                table: "Structures",
                type: "varchar(16)",
                maxLength: 16,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 0L)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Structures",
                keyColumn: "Id",
                keyValue: -1L,
                column: "C59Struttura",
                value: "");
        }
    }
}
