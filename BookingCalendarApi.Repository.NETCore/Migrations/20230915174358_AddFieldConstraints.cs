using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Repository.NETCore.Migrations
{
    public partial class AddFieldConstraints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRefreshTokens",
                table: "UserRefreshTokens");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "varchar(128)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql("delete from UserRefreshTokens;");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "UserRefreshTokens",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "UserRefreshTokens",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "UserRefreshTokens",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Rooms",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Rooms",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "RoomAssignments",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Nations",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<long>(
                name: "Code",
                table: "Nations",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bigint unsigned");

            migrationBuilder.AlterColumn<string>(
                name: "Iso",
                table: "Nations",
                type: "varchar(2)",
                maxLength: 2,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Floors",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "ColorAssignments",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "ColorAssignments",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRefreshTokens",
                table: "UserRefreshTokens",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AD",
                column: "Code",
                value: 100000202L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AE",
                column: "Code",
                value: 100000322L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AF",
                column: "Code",
                value: 100000301L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AG",
                column: "Code",
                value: 100000503L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AI",
                column: "Code",
                value: 100000402L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AL",
                column: "Code",
                value: 100000201L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AM",
                column: "Code",
                value: 100000358L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AO",
                column: "Code",
                value: 100000402L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AQ",
                column: "Code",
                value: 100000733L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AR",
                column: "Code",
                value: 100000602L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AS",
                column: "Code",
                value: 100000798L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AT",
                column: "Code",
                value: 100000203L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AU",
                column: "Code",
                value: 100000701L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AW",
                column: "Code",
                value: 100000358L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AX",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AZ",
                column: "Code",
                value: 100000359L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BA",
                column: "Code",
                value: 100000252L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BB",
                column: "Code",
                value: 100000506L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BD",
                column: "Code",
                value: 100000305L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BE",
                column: "Code",
                value: 100000206L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BF",
                column: "Code",
                value: 100000409L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BG",
                column: "Code",
                value: 100000209L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BH",
                column: "Code",
                value: 100000304L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BI",
                column: "Code",
                value: 100000410L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BJ",
                column: "Code",
                value: 100000406L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BL",
                column: "Code",
                value: 100000797L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BM",
                column: "Code",
                value: 100000406L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BN",
                column: "Code",
                value: 100000309L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BO",
                column: "Code",
                value: 100000604L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BQ",
                column: "Code",
                value: 100000232L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BR",
                column: "Code",
                value: 100000605L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BS",
                column: "Code",
                value: 100000505L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BT",
                column: "Code",
                value: 100000306L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BV",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BW",
                column: "Code",
                value: 100000408L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BY",
                column: "Code",
                value: 100000256L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BZ",
                column: "Code",
                value: 100000507L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CA",
                column: "Code",
                value: 100000509L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CC",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CD",
                column: "Code",
                value: 100000998L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CF",
                column: "Code",
                value: 100000257L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CG",
                column: "Code",
                value: 100000257L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CH",
                column: "Code",
                value: 100000241L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CI",
                column: "Code",
                value: 100000404L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CK",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CL",
                column: "Code",
                value: 100000606L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CM",
                column: "Code",
                value: 100000411L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CN",
                column: "Code",
                value: 100000606L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CO",
                column: "Code",
                value: 100000608L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CR",
                column: "Code",
                value: 100000404L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CU",
                column: "Code",
                value: 100000514L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CV",
                column: "Code",
                value: 100000413L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CW",
                column: "Code",
                value: 100000514L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CX",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CY",
                column: "Code",
                value: 100000315L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CZ",
                column: "Code",
                value: 100000257L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DE",
                column: "Code",
                value: 100000216L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DJ",
                column: "Code",
                value: 100000424L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DK",
                column: "Code",
                value: 100000212L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DM",
                column: "Code",
                value: 100000515L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DO",
                column: "Code",
                value: 100000997L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DZ",
                column: "Code",
                value: 100000401L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "EC",
                column: "Code",
                value: 100000609L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "EE",
                column: "Code",
                value: 100000247L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "EG",
                column: "Code",
                value: 100000419L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "EH",
                column: "Code",
                value: 100000533L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ER",
                column: "Code",
                value: 100000466L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ES",
                column: "Code",
                value: 100000239L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ET",
                column: "Code",
                value: 100000420L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FI",
                column: "Code",
                value: 100000214L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FJ",
                column: "Code",
                value: 100000703L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FK",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FM",
                column: "Code",
                value: 100000311L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FO",
                column: "Code",
                value: 100000755L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FR",
                column: "Code",
                value: 100000215L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GA",
                column: "Code",
                value: 100000421L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GB",
                column: "Code",
                value: 100000219L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GD",
                column: "Code",
                value: 100000519L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GE",
                column: "Code",
                value: 100000360L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GF",
                column: "Code",
                value: 100000612L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GG",
                column: "Code",
                value: 100000761L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GH",
                column: "Code",
                value: 100000423L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GI",
                column: "Code",
                value: 100000326L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GL",
                column: "Code",
                value: 100000758L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GM",
                column: "Code",
                value: 100000422L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GN",
                column: "Code",
                value: 100000425L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GP",
                column: "Code",
                value: 100000759L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GQ",
                column: "Code",
                value: 100000427L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GR",
                column: "Code",
                value: 100000220L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GS",
                column: "Code",
                value: 100000360L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GT",
                column: "Code",
                value: 100000523L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GU",
                column: "Code",
                value: 100000760L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GW",
                column: "Code",
                value: 100000427L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GY",
                column: "Code",
                value: 100000612L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HK",
                column: "Code",
                value: 110000005L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HM",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HN",
                column: "Code",
                value: 100000525L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HR",
                column: "Code",
                value: 100000250L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HT",
                column: "Code",
                value: 100000524L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HU",
                column: "Code",
                value: 100000244L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ID",
                column: "Code",
                value: 100000331L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IE",
                column: "Code",
                value: 100000221L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IL",
                column: "Code",
                value: 100000334L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IM",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IN",
                column: "Code",
                value: 100000330L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IO",
                column: "Code",
                value: 100000457L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IQ",
                column: "Code",
                value: 100000333L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IR",
                column: "Code",
                value: 100000332L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IS",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IT",
                column: "Code",
                value: 100000100L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "JE",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "JM",
                column: "Code",
                value: 100000518L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "JO",
                column: "Code",
                value: 100000327L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "JP",
                column: "Code",
                value: 100000326L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KE",
                column: "Code",
                value: 100000428L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KG",
                column: "Code",
                value: 100000361L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KH",
                column: "Code",
                value: 100000310L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KI",
                column: "Code",
                value: 100000708L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KM",
                column: "Code",
                value: 100000417L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KN",
                column: "Code",
                value: 100000795L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KP",
                column: "Code",
                value: 100000319L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KR",
                column: "Code",
                value: 100000320L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KW",
                column: "Code",
                value: 100000335L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KY",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KZ",
                column: "Code",
                value: 100000356L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LA",
                column: "Code",
                value: 100000336L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LB",
                column: "Code",
                value: 100000337L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LC",
                column: "Code",
                value: 100000532L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LI",
                column: "Code",
                value: 100000225L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LK",
                column: "Code",
                value: 100000239L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LR",
                column: "Code",
                value: 100000430L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LS",
                column: "Code",
                value: 100000429L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LT",
                column: "Code",
                value: 100000249L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LU",
                column: "Code",
                value: 100000226L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LV",
                column: "Code",
                value: 100000248L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LY",
                column: "Code",
                value: 100000431L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MA",
                column: "Code",
                value: 100000436L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MC",
                column: "Code",
                value: 100000234L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MD",
                column: "Code",
                value: 100000254L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ME",
                column: "Code",
                value: 100001001L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MF",
                column: "Code",
                value: 100000797L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MG",
                column: "Code",
                value: 100000432L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MH",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MK",
                column: "Code",
                value: 100000253L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ML",
                column: "Code",
                value: 100000435L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MM",
                column: "Code",
                value: 100000256L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MN",
                column: "Code",
                value: 100000341L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MO",
                column: "Code",
                value: 110000003L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MP",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MQ",
                column: "Code",
                value: 100000773L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MR",
                column: "Code",
                value: 100000437L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MS",
                column: "Code",
                value: 100000777L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MT",
                column: "Code",
                value: 100000227L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MU",
                column: "Code",
                value: 100000437L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MV",
                column: "Code",
                value: 100000339L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MW",
                column: "Code",
                value: 100000434L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MX",
                column: "Code",
                value: 100000527L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MY",
                column: "Code",
                value: 100000767L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MZ",
                column: "Code",
                value: 100000440L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NA",
                column: "Code",
                value: 100000441L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NC",
                column: "Code",
                value: 100000780L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NE",
                column: "Code",
                value: 100000442L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NF",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NG",
                column: "Code",
                value: 100000443L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NI",
                column: "Code",
                value: 100000529L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NL",
                column: "Code",
                value: 100000232L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NO",
                column: "Code",
                value: 100000231L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NP",
                column: "Code",
                value: 100000342L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NR",
                column: "Code",
                value: 100000715L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NU",
                column: "Code",
                value: 100000443L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NZ",
                column: "Code",
                value: 100000719L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "OM",
                column: "Code",
                value: 100000343L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PA",
                column: "Code",
                value: 100000530L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PE",
                column: "Code",
                value: 100000615L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PF",
                column: "Code",
                value: 100000787L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PG",
                column: "Code",
                value: 100000530L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PH",
                column: "Code",
                value: 100000323L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PK",
                column: "Code",
                value: 100000344L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PL",
                column: "Code",
                value: 100000233L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PM",
                column: "Code",
                value: 100000797L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PN",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PR",
                column: "Code",
                value: 100000233L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PS",
                column: "Code",
                value: 100000536L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PT",
                column: "Code",
                value: 100000234L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PW",
                column: "Code",
                value: 100000344L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PY",
                column: "Code",
                value: 100000614L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "QA",
                column: "Code",
                value: 100000345L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RE",
                column: "Code",
                value: 100000765L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RO",
                column: "Code",
                value: 100000235L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RS",
                column: "Code",
                value: 100001000L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RU",
                column: "Code",
                value: 100000245L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RW",
                column: "Code",
                value: 100000446L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SA",
                column: "Code",
                value: 100000302L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SB",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SC",
                column: "Code",
                value: 100001000L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SD",
                column: "Code",
                value: 100000455L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SE",
                column: "Code",
                value: 100000240L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SG",
                column: "Code",
                value: 100000346L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SH",
                column: "Code",
                value: 100000799L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SI",
                column: "Code",
                value: 100000251L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SJ",
                column: "Code",
                value: 100000616L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SK",
                column: "Code",
                value: 100000348L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SL",
                column: "Code",
                value: 100000451L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SM",
                column: "Code",
                value: 100000236L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SN",
                column: "Code",
                value: 100000450L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SO",
                column: "Code",
                value: 100000453L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SR",
                column: "Code",
                value: 100000616L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SS",
                column: "Code",
                value: 100000455L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ST",
                column: "Code",
                value: 100000448L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SV",
                column: "Code",
                value: 100000517L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SX",
                column: "Code",
                value: 100000346L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SY",
                column: "Code",
                value: 100000348L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SZ",
                column: "Code",
                value: 100000456L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TC",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TD",
                column: "Code",
                value: 100000415L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TF",
                column: "Code",
                value: 100000457L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TG",
                column: "Code",
                value: 100000458L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TH",
                column: "Code",
                value: 100000349L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TJ",
                column: "Code",
                value: 100000362L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TK",
                column: "Code",
                value: 100000806L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TL",
                column: "Code",
                value: 100000805L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TM",
                column: "Code",
                value: 100000364L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TN",
                column: "Code",
                value: 100000460L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TO",
                column: "Code",
                value: 100000730L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TR",
                column: "Code",
                value: 100000351L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TT",
                column: "Code",
                value: 100000617L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TV",
                column: "Code",
                value: 100000731L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TW",
                column: "Code",
                value: 100000998L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TZ",
                column: "Code",
                value: 100000457L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UA",
                column: "Code",
                value: 100000243L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UG",
                column: "Code",
                value: 100000461L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UK",
                column: "Code",
                value: 100000219L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UM",
                column: "Code",
                value: 100000223L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "US",
                column: "Code",
                value: 100000536L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UY",
                column: "Code",
                value: 100000618L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UZ",
                column: "Code",
                value: 100000357L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VA",
                column: "Code",
                value: 100000246L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VC",
                column: "Code",
                value: 100000797L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VE",
                column: "Code",
                value: 100000619L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VG",
                column: "Code",
                value: 100000764L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VI",
                column: "Code",
                value: 100000764L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VN",
                column: "Code",
                value: 100000353L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VU",
                column: "Code",
                value: 100000732L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "WF",
                column: "Code",
                value: 100000815L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "WS",
                column: "Code",
                value: 100000727L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "YE",
                column: "Code",
                value: 100000354L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "YT",
                column: "Code",
                value: 100000774L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ZA",
                column: "Code",
                value: 100000467L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ZM",
                column: "Code",
                value: 100000464L);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ZW",
                column: "Code",
                value: 100000465L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRefreshTokens",
                table: "UserRefreshTokens");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserRefreshTokens");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "UserRefreshTokens",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "UserRefreshTokens",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Rooms",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "Rooms",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)",
                oldMaxLength: 10)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "RoomAssignments",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Nations",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<ulong>(
                name: "Code",
                table: "Nations",
                type: "bigint unsigned",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Iso",
                table: "Nations",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(2)",
                oldMaxLength: 2)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Floors",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "ColorAssignments",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "BookingId",
                table: "ColorAssignments",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRefreshTokens",
                table: "UserRefreshTokens",
                column: "RefreshToken");

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AD",
                column: "Code",
                value: 100000202ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AE",
                column: "Code",
                value: 100000322ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AF",
                column: "Code",
                value: 100000301ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AG",
                column: "Code",
                value: 100000503ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AI",
                column: "Code",
                value: 100000402ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AL",
                column: "Code",
                value: 100000201ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AM",
                column: "Code",
                value: 100000358ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AO",
                column: "Code",
                value: 100000402ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AQ",
                column: "Code",
                value: 100000733ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AR",
                column: "Code",
                value: 100000602ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AS",
                column: "Code",
                value: 100000798ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AT",
                column: "Code",
                value: 100000203ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AU",
                column: "Code",
                value: 100000701ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AW",
                column: "Code",
                value: 100000358ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AX",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "AZ",
                column: "Code",
                value: 100000359ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BA",
                column: "Code",
                value: 100000252ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BB",
                column: "Code",
                value: 100000506ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BD",
                column: "Code",
                value: 100000305ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BE",
                column: "Code",
                value: 100000206ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BF",
                column: "Code",
                value: 100000409ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BG",
                column: "Code",
                value: 100000209ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BH",
                column: "Code",
                value: 100000304ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BI",
                column: "Code",
                value: 100000410ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BJ",
                column: "Code",
                value: 100000406ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BL",
                column: "Code",
                value: 100000797ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BM",
                column: "Code",
                value: 100000406ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BN",
                column: "Code",
                value: 100000309ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BO",
                column: "Code",
                value: 100000604ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BQ",
                column: "Code",
                value: 100000232ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BR",
                column: "Code",
                value: 100000605ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BS",
                column: "Code",
                value: 100000505ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BT",
                column: "Code",
                value: 100000306ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BV",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BW",
                column: "Code",
                value: 100000408ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BY",
                column: "Code",
                value: 100000256ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "BZ",
                column: "Code",
                value: 100000507ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CA",
                column: "Code",
                value: 100000509ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CC",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CD",
                column: "Code",
                value: 100000998ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CF",
                column: "Code",
                value: 100000257ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CG",
                column: "Code",
                value: 100000257ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CH",
                column: "Code",
                value: 100000241ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CI",
                column: "Code",
                value: 100000404ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CK",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CL",
                column: "Code",
                value: 100000606ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CM",
                column: "Code",
                value: 100000411ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CN",
                column: "Code",
                value: 100000606ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CO",
                column: "Code",
                value: 100000608ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CR",
                column: "Code",
                value: 100000404ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CU",
                column: "Code",
                value: 100000514ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CV",
                column: "Code",
                value: 100000413ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CW",
                column: "Code",
                value: 100000514ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CX",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CY",
                column: "Code",
                value: 100000315ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "CZ",
                column: "Code",
                value: 100000257ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DE",
                column: "Code",
                value: 100000216ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DJ",
                column: "Code",
                value: 100000424ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DK",
                column: "Code",
                value: 100000212ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DM",
                column: "Code",
                value: 100000515ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DO",
                column: "Code",
                value: 100000997ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DZ",
                column: "Code",
                value: 100000401ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "EC",
                column: "Code",
                value: 100000609ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "EE",
                column: "Code",
                value: 100000247ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "EG",
                column: "Code",
                value: 100000419ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "EH",
                column: "Code",
                value: 100000533ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ER",
                column: "Code",
                value: 100000466ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ES",
                column: "Code",
                value: 100000239ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ET",
                column: "Code",
                value: 100000420ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FI",
                column: "Code",
                value: 100000214ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FJ",
                column: "Code",
                value: 100000703ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FK",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FM",
                column: "Code",
                value: 100000311ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FO",
                column: "Code",
                value: 100000755ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "FR",
                column: "Code",
                value: 100000215ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GA",
                column: "Code",
                value: 100000421ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GB",
                column: "Code",
                value: 100000219ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GD",
                column: "Code",
                value: 100000519ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GE",
                column: "Code",
                value: 100000360ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GF",
                column: "Code",
                value: 100000612ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GG",
                column: "Code",
                value: 100000761ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GH",
                column: "Code",
                value: 100000423ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GI",
                column: "Code",
                value: 100000326ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GL",
                column: "Code",
                value: 100000758ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GM",
                column: "Code",
                value: 100000422ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GN",
                column: "Code",
                value: 100000425ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GP",
                column: "Code",
                value: 100000759ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GQ",
                column: "Code",
                value: 100000427ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GR",
                column: "Code",
                value: 100000220ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GS",
                column: "Code",
                value: 100000360ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GT",
                column: "Code",
                value: 100000523ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GU",
                column: "Code",
                value: 100000760ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GW",
                column: "Code",
                value: 100000427ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "GY",
                column: "Code",
                value: 100000612ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HK",
                column: "Code",
                value: 110000005ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HM",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HN",
                column: "Code",
                value: 100000525ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HR",
                column: "Code",
                value: 100000250ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HT",
                column: "Code",
                value: 100000524ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "HU",
                column: "Code",
                value: 100000244ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ID",
                column: "Code",
                value: 100000331ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IE",
                column: "Code",
                value: 100000221ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IL",
                column: "Code",
                value: 100000334ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IM",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IN",
                column: "Code",
                value: 100000330ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IO",
                column: "Code",
                value: 100000457ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IQ",
                column: "Code",
                value: 100000333ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IR",
                column: "Code",
                value: 100000332ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IS",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "IT",
                column: "Code",
                value: 100000100ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "JE",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "JM",
                column: "Code",
                value: 100000518ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "JO",
                column: "Code",
                value: 100000327ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "JP",
                column: "Code",
                value: 100000326ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KE",
                column: "Code",
                value: 100000428ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KG",
                column: "Code",
                value: 100000361ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KH",
                column: "Code",
                value: 100000310ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KI",
                column: "Code",
                value: 100000708ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KM",
                column: "Code",
                value: 100000417ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KN",
                column: "Code",
                value: 100000795ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KP",
                column: "Code",
                value: 100000319ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KR",
                column: "Code",
                value: 100000320ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KW",
                column: "Code",
                value: 100000335ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KY",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "KZ",
                column: "Code",
                value: 100000356ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LA",
                column: "Code",
                value: 100000336ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LB",
                column: "Code",
                value: 100000337ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LC",
                column: "Code",
                value: 100000532ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LI",
                column: "Code",
                value: 100000225ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LK",
                column: "Code",
                value: 100000239ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LR",
                column: "Code",
                value: 100000430ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LS",
                column: "Code",
                value: 100000429ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LT",
                column: "Code",
                value: 100000249ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LU",
                column: "Code",
                value: 100000226ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LV",
                column: "Code",
                value: 100000248ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "LY",
                column: "Code",
                value: 100000431ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MA",
                column: "Code",
                value: 100000436ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MC",
                column: "Code",
                value: 100000234ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MD",
                column: "Code",
                value: 100000254ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ME",
                column: "Code",
                value: 100001001ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MF",
                column: "Code",
                value: 100000797ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MG",
                column: "Code",
                value: 100000432ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MH",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MK",
                column: "Code",
                value: 100000253ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ML",
                column: "Code",
                value: 100000435ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MM",
                column: "Code",
                value: 100000256ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MN",
                column: "Code",
                value: 100000341ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MO",
                column: "Code",
                value: 110000003ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MP",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MQ",
                column: "Code",
                value: 100000773ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MR",
                column: "Code",
                value: 100000437ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MS",
                column: "Code",
                value: 100000777ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MT",
                column: "Code",
                value: 100000227ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MU",
                column: "Code",
                value: 100000437ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MV",
                column: "Code",
                value: 100000339ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MW",
                column: "Code",
                value: 100000434ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MX",
                column: "Code",
                value: 100000527ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MY",
                column: "Code",
                value: 100000767ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MZ",
                column: "Code",
                value: 100000440ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NA",
                column: "Code",
                value: 100000441ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NC",
                column: "Code",
                value: 100000780ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NE",
                column: "Code",
                value: 100000442ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NF",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NG",
                column: "Code",
                value: 100000443ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NI",
                column: "Code",
                value: 100000529ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NL",
                column: "Code",
                value: 100000232ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NO",
                column: "Code",
                value: 100000231ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NP",
                column: "Code",
                value: 100000342ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NR",
                column: "Code",
                value: 100000715ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NU",
                column: "Code",
                value: 100000443ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "NZ",
                column: "Code",
                value: 100000719ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "OM",
                column: "Code",
                value: 100000343ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PA",
                column: "Code",
                value: 100000530ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PE",
                column: "Code",
                value: 100000615ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PF",
                column: "Code",
                value: 100000787ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PG",
                column: "Code",
                value: 100000530ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PH",
                column: "Code",
                value: 100000323ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PK",
                column: "Code",
                value: 100000344ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PL",
                column: "Code",
                value: 100000233ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PM",
                column: "Code",
                value: 100000797ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PN",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PR",
                column: "Code",
                value: 100000233ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PS",
                column: "Code",
                value: 100000536ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PT",
                column: "Code",
                value: 100000234ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PW",
                column: "Code",
                value: 100000344ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PY",
                column: "Code",
                value: 100000614ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "QA",
                column: "Code",
                value: 100000345ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RE",
                column: "Code",
                value: 100000765ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RO",
                column: "Code",
                value: 100000235ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RS",
                column: "Code",
                value: 100001000ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RU",
                column: "Code",
                value: 100000245ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "RW",
                column: "Code",
                value: 100000446ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SA",
                column: "Code",
                value: 100000302ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SB",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SC",
                column: "Code",
                value: 100001000ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SD",
                column: "Code",
                value: 100000455ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SE",
                column: "Code",
                value: 100000240ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SG",
                column: "Code",
                value: 100000346ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SH",
                column: "Code",
                value: 100000799ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SI",
                column: "Code",
                value: 100000251ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SJ",
                column: "Code",
                value: 100000616ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SK",
                column: "Code",
                value: 100000348ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SL",
                column: "Code",
                value: 100000451ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SM",
                column: "Code",
                value: 100000236ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SN",
                column: "Code",
                value: 100000450ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SO",
                column: "Code",
                value: 100000453ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SR",
                column: "Code",
                value: 100000616ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SS",
                column: "Code",
                value: 100000455ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ST",
                column: "Code",
                value: 100000448ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SV",
                column: "Code",
                value: 100000517ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SX",
                column: "Code",
                value: 100000346ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SY",
                column: "Code",
                value: 100000348ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SZ",
                column: "Code",
                value: 100000456ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TC",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TD",
                column: "Code",
                value: 100000415ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TF",
                column: "Code",
                value: 100000457ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TG",
                column: "Code",
                value: 100000458ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TH",
                column: "Code",
                value: 100000349ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TJ",
                column: "Code",
                value: 100000362ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TK",
                column: "Code",
                value: 100000806ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TL",
                column: "Code",
                value: 100000805ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TM",
                column: "Code",
                value: 100000364ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TN",
                column: "Code",
                value: 100000460ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TO",
                column: "Code",
                value: 100000730ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TR",
                column: "Code",
                value: 100000351ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TT",
                column: "Code",
                value: 100000617ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TV",
                column: "Code",
                value: 100000731ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TW",
                column: "Code",
                value: 100000998ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "TZ",
                column: "Code",
                value: 100000457ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UA",
                column: "Code",
                value: 100000243ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UG",
                column: "Code",
                value: 100000461ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UK",
                column: "Code",
                value: 100000219ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UM",
                column: "Code",
                value: 100000223ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "US",
                column: "Code",
                value: 100000536ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UY",
                column: "Code",
                value: 100000618ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UZ",
                column: "Code",
                value: 100000357ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VA",
                column: "Code",
                value: 100000246ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VC",
                column: "Code",
                value: 100000797ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VE",
                column: "Code",
                value: 100000619ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VG",
                column: "Code",
                value: 100000764ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VI",
                column: "Code",
                value: 100000764ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VN",
                column: "Code",
                value: 100000353ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "VU",
                column: "Code",
                value: 100000732ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "WF",
                column: "Code",
                value: 100000815ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "WS",
                column: "Code",
                value: 100000727ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "YE",
                column: "Code",
                value: 100000354ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "YT",
                column: "Code",
                value: 100000774ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ZA",
                column: "Code",
                value: 100000467ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ZM",
                column: "Code",
                value: 100000464ul);

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "ZW",
                column: "Code",
                value: 100000465ul);
        }
    }
}
