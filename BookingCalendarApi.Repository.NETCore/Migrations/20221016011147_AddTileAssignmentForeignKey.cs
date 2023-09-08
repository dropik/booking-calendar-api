using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Repository.NETCore.Migrations
{
    public partial class AddTileAssignmentForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TileAssignments_RoomId",
                table: "TileAssignments",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_TileAssignments_Rooms_RoomId",
                table: "TileAssignments",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TileAssignments_Rooms_RoomId",
                table: "TileAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TileAssignments_RoomId",
                table: "TileAssignments");
        }
    }
}
