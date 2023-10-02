using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Repository.NETCore.Migrations
{
    public partial class AddConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoomAssignments",
                keyColumn: "RoomId",
                keyValue: null);

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAssignments_Rooms_RoomId",
                table: "RoomAssignments");

            migrationBuilder.AlterColumn<long>(
                name: "RoomId",
                table: "RoomAssignments",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAssignments_Rooms_RoomId",
                table: "RoomAssignments",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoomAssignments_Rooms_RoomId",
                table: "RoomAssignments");

            migrationBuilder.AlterColumn<long>(
                name: "RoomId",
                table: "RoomAssignments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAssignments_Rooms_RoomId",
                table: "RoomAssignments",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
