using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingCalendarApi.NETFramework.Migrations
{
    public partial class LinkEverythingToStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StructureId",
                table: "Rooms",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<long>(
                name: "StructureId",
                table: "RoomAssignments",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<long>(
                name: "StructureId",
                table: "Floors",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<long>(
                name: "StructureId",
                table: "ColorAssignments",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_StructureId",
                table: "Rooms",
                column: "StructureId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomAssignments_StructureId",
                table: "RoomAssignments",
                column: "StructureId");

            migrationBuilder.CreateIndex(
                name: "IX_Floors_StructureId",
                table: "Floors",
                column: "StructureId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorAssignments_StructureId",
                table: "ColorAssignments",
                column: "StructureId");

            migrationBuilder.AddForeignKey(
                name: "FK_ColorAssignments_Structures_StructureId",
                table: "ColorAssignments",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Floors_Structures_StructureId",
                table: "Floors",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoomAssignments_Structures_StructureId",
                table: "RoomAssignments",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Structures_StructureId",
                table: "Rooms",
                column: "StructureId",
                principalTable: "Structures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ColorAssignments_Structures_StructureId",
                table: "ColorAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Floors_Structures_StructureId",
                table: "Floors");

            migrationBuilder.DropForeignKey(
                name: "FK_RoomAssignments_Structures_StructureId",
                table: "RoomAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Structures_StructureId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_StructureId",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_RoomAssignments_StructureId",
                table: "RoomAssignments");

            migrationBuilder.DropIndex(
                name: "IX_Floors_StructureId",
                table: "Floors");

            migrationBuilder.DropIndex(
                name: "IX_ColorAssignments_StructureId",
                table: "ColorAssignments");

            migrationBuilder.DropColumn(
                name: "StructureId",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "StructureId",
                table: "RoomAssignments");

            migrationBuilder.DropColumn(
                name: "StructureId",
                table: "Floors");

            migrationBuilder.DropColumn(
                name: "StructureId",
                table: "ColorAssignments");
        }
    }
}
