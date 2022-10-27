using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Migrations
{
    public partial class AddNationDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoliceNations");

            migrationBuilder.CreateTable(
                name: "Nations",
                columns: table => new
                {
                    Iso = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<ulong>(type: "bigint unsigned", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nations", x => x.Iso);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nations");

            migrationBuilder.CreateTable(
                name: "PoliceNations",
                columns: table => new
                {
                    Iso = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoliceNations", x => x.Iso);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
