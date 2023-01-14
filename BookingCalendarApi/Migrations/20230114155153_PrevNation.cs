using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Migrations
{
    public partial class PrevNation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Nations");

            migrationBuilder.CreateTable(
                name: "PrevNations",
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
                    table.PrimaryKey("PK_PrevNations", x => x.Iso);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "PrevNations",
                columns: new[] { "Iso", "Code", "Description" },
                values: new object[] { "UK", 100000219ul, "REGNO UNITO" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrevNations");

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

            migrationBuilder.InsertData(
                table: "Nations",
                columns: new[] { "Iso", "Code", "Description" },
                values: new object[] { "UK", 100000219ul, "REGNO UNITO" });
        }
    }
}
