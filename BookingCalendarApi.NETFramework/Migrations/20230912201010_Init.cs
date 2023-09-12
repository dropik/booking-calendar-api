using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingCalendarApi.NETFramework.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColorAssignments",
                columns: table => new
                {
                    BookingId = table.Column<string>(nullable: false),
                    Color = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorAssignments", x => x.BookingId);
                });

            migrationBuilder.CreateTable(
                name: "Floors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Floors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nations",
                columns: table => new
                {
                    Iso = table.Column<string>(nullable: false),
                    Code = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nations", x => x.Iso);
                });

            migrationBuilder.CreateTable(
                name: "Structures",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Structures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRefreshTokens",
                columns: table => new
                {
                    RefreshToken = table.Column<string>(nullable: false),
                    Username = table.Column<string>(nullable: false),
                    ExpiresAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRefreshTokens", x => x.RefreshToken);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FloorId = table.Column<long>(nullable: false),
                    Number = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Floors_FloorId",
                        column: x => x.FloorId,
                        principalTable: "Floors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StructureId = table.Column<long>(nullable: false),
                    Username = table.Column<string>(maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(nullable: false),
                    VisibleName = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Structures_StructureId",
                        column: x => x.StructureId,
                        principalTable: "Structures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomAssignments",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    RoomId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoomAssignments_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Nations",
                columns: new[] { "Iso", "Code", "Description" },
                values: new object[,]
                {
                    { "AD", 100000202L, "ANDORRA" },
                    { "MZ", 100000440L, "MOZAMBICO" },
                    { "NA", 100000441L, "NAMIBIA" },
                    { "NC", 100000780L, "FRANCIA" },
                    { "NE", 100000442L, "NIGER" },
                    { "NF", 100000223L, "AUSTRALIA" },
                    { "NG", 100000443L, "NIGERIA" },
                    { "NI", 100000529L, "NICARAGUA" },
                    { "NL", 100000232L, "PAESI BASSI" },
                    { "NO", 100000231L, "NORVEGIA" },
                    { "NP", 100000342L, "NEPAL" },
                    { "NR", 100000715L, "NAURU" },
                    { "NU", 100000443L, "NUOVA ZELANDA" },
                    { "NZ", 100000719L, "NUOVA ZELANDA" },
                    { "OM", 100000343L, "OMAN" },
                    { "PA", 100000530L, "PANAMA" },
                    { "PE", 100000615L, "PERU'" },
                    { "PF", 100000787L, "FRANCIA" },
                    { "PG", 100000530L, "PAPUA NUOVA GUINEA" },
                    { "PH", 100000323L, "FILIPPINE" },
                    { "PK", 100000344L, "PAKISTAN" },
                    { "PL", 100000233L, "POLONIA" },
                    { "PM", 100000797L, "FRANCIA" },
                    { "PN", 100000223L, "REGNO UNITO" },
                    { "PR", 100000233L, "STATI UNITI D'AMERICA" },
                    { "PS", 100000536L, "PAKISTAN" },
                    { "PT", 100000234L, "PORTOGALLO" },
                    { "PW", 100000344L, "PALAU" },
                    { "MY", 100000767L, "MALAYSIA" },
                    { "PY", 100000614L, "PARAGUAY" },
                    { "MX", 100000527L, "MESSICO" },
                    { "MV", 100000339L, "MALDIVE" },
                    { "LC", 100000532L, "SAINT LUCIA" },
                    { "LI", 100000225L, "LIECHTENSTEIN" },
                    { "LK", 100000239L, "SRI LANKA (CEYLON)" },
                    { "LR", 100000430L, "LIBERIA" },
                    { "LS", 100000429L, "LESOTHO" },
                    { "LT", 100000249L, "LITUANIA" },
                    { "LU", 100000226L, "LUSSEMBURGO" },
                    { "LV", 100000248L, "LETTONIA" },
                    { "LY", 100000431L, "LIBIA" },
                    { "MA", 100000436L, "MAROCCO" },
                    { "MC", 100000234L, "MONACO" },
                    { "MD", 100000254L, "MOLDOVA" },
                    { "ME", 100001001L, "MONTENEGRO" },
                    { "MF", 100000797L, "FRANCIA" },
                    { "MG", 100000432L, "MADAGASCAR" },
                    { "MH", 100000223L, "MARSHALL" },
                    { "MK", 100000253L, "MACEDONIA (EX REPUBBLICA JUGOSLAVA)" },
                    { "ML", 100000435L, "MALI" },
                    { "MM", 100000256L, "MYANMAR" },
                    { "MN", 100000341L, "MONGOLIA" },
                    { "MO", 110000003L, "REPUBBLICA POPOLARE CINESE" },
                    { "MP", 100000223L, "STATI UNITI D'AMERICA" },
                    { "MQ", 100000773L, "FRANCIA" },
                    { "MR", 100000437L, "MAURITANIA" },
                    { "MS", 100000777L, "REGNO UNITO" },
                    { "MT", 100000227L, "MALTA" },
                    { "MU", 100000437L, "MAURITIUS" },
                    { "MW", 100000434L, "MALAWI" },
                    { "QA", 100000345L, "QATAR" },
                    { "RE", 100000765L, "FRANCIA" },
                    { "RO", 100000235L, "ROMANIA" },
                    { "TM", 100000364L, "TURKMENISTAN" },
                    { "TN", 100000460L, "TUNISIA" },
                    { "TO", 100000730L, "TONGA" },
                    { "TR", 100000351L, "TURCHIA" },
                    { "TT", 100000617L, "TRINIDAD E TOBAGO" },
                    { "TV", 100000731L, "TUVALU" },
                    { "TW", 100000998L, "TAIWAN" },
                    { "TZ", 100000457L, "TANZANIA" },
                    { "UA", 100000243L, "UCRAINA" },
                    { "UG", 100000461L, "UGANDA" },
                    { "UK", 100000219L, "REGNO UNITO" },
                    { "UM", 100000223L, "STATI UNITI D'AMERICA" },
                    { "US", 100000536L, "STATI UNITI D'AMERICA" },
                    { "UY", 100000618L, "URUGUAY" },
                    { "UZ", 100000357L, "UZBEKISTAN" },
                    { "VA", 100000246L, "SANTA SEDE" },
                    { "VC", 100000797L, "S. VINCENT E GRENADINE" },
                    { "VE", 100000619L, "VENEZUELA" },
                    { "VG", 100000764L, "REGNO UNITO" },
                    { "VI", 100000764L, "STATI UNITI D'AMERICA" },
                    { "VN", 100000353L, "VIETNAM" },
                    { "VU", 100000732L, "VANUATU" },
                    { "WF", 100000815L, "FRANCIA" },
                    { "WS", 100000727L, "SAMOA" },
                    { "YE", 100000354L, "YEMEN" },
                    { "YT", 100000774L, "FRANCIA" },
                    { "ZA", 100000467L, "SUD AFRICA" },
                    { "TL", 100000805L, "TIMOR ORIENTALE" },
                    { "TK", 100000806L, "NUOVA ZELANDA" },
                    { "TJ", 100000362L, "TAGIKISTAN" },
                    { "TH", 100000349L, "THAILANDIA" },
                    { "RS", 100001000L, "SERBIA REPUBBLICA DI" },
                    { "RU", 100000245L, "FEDERAZIONE RUSSA" },
                    { "RW", 100000446L, "RUANDA" },
                    { "SA", 100000302L, "ARABIA SAUDITA" },
                    { "SB", 100000223L, "SALOMONE" },
                    { "SC", 100001000L, "SEYCHELLES" },
                    { "SD", 100000455L, "SUDAN" },
                    { "SE", 100000240L, "SVEZIA" },
                    { "SG", 100000346L, "SINGAPORE" },
                    { "SH", 100000799L, "REGNO UNITO" },
                    { "SI", 100000251L, "SLOVENIA" },
                    { "SJ", 100000616L, "NORVEGIA" },
                    { "SK", 100000348L, "REPUBBLICA SLOVACCA" },
                    { "LB", 100000337L, "LIBANO" },
                    { "SL", 100000451L, "SIERRA LEONE" },
                    { "SN", 100000450L, "SENEGAL" },
                    { "SO", 100000453L, "SOMALIA" },
                    { "SR", 100000616L, "SURINAME" },
                    { "SS", 100000455L, "SUD SUDAN, REPUBBLICA DEL" },
                    { "ST", 100000448L, "SAO TOME' E PRINCIPE" },
                    { "SV", 100000517L, "EL SALVADOR" },
                    { "SX", 100000346L, "PAESI BASSI" },
                    { "SY", 100000348L, "SIRIA" },
                    { "SZ", 100000456L, "SWAZILAND" },
                    { "TC", 100000223L, "REGNO UNITO" },
                    { "TD", 100000415L, "CIAD" },
                    { "TF", 100000457L, "FRANCIA" },
                    { "TG", 100000458L, "TOGO" },
                    { "SM", 100000236L, "SAN MARINO" },
                    { "LA", 100000336L, "LAOS" },
                    { "KZ", 100000356L, "KAZAKISTAN" },
                    { "KY", 100000223L, "REGNO UNITO" },
                    { "BV", 100000223L, "NORVEGIA" },
                    { "BW", 100000408L, "BOTSWANA" },
                    { "BY", 100000256L, "BIELORUSSIA" },
                    { "BZ", 100000507L, "BELIZE" },
                    { "CA", 100000509L, "CANADA" },
                    { "CC", 100000223L, "AUSTRALIA" },
                    { "CD", 100000998L, "REPUBBLICA DEMOCRATICA DEL CONGO (EX ZAIRE)" },
                    { "CF", 100000257L, "REPUBBLICA CENTRAFRICANA" },
                    { "CG", 100000257L, "CONGO" },
                    { "CH", 100000241L, "SVIZZERA" },
                    { "CI", 100000404L, "COSTA D'AVORIO" },
                    { "CK", 100000223L, "NUOVA ZELANDA" },
                    { "CL", 100000606L, "CILE" },
                    { "CM", 100000411L, "CAMERUN" },
                    { "CN", 100000606L, "REPUBBLICA POPOLARE CINESE" },
                    { "CO", 100000608L, "COLOMBIA" },
                    { "CR", 100000404L, "COSTARICA" },
                    { "CU", 100000514L, "CUBA" },
                    { "CV", 100000413L, "CAPO VERDE" },
                    { "CW", 100000514L, "PAESI BASSI" },
                    { "CX", 100000223L, "AUSTRALIA" },
                    { "CY", 100000315L, "CIPRO" },
                    { "CZ", 100000257L, "REPUBBLICA CECA" },
                    { "DE", 100000216L, "GERMANIA" },
                    { "DJ", 100000424L, "GIBUTI" },
                    { "DK", 100000212L, "DANIMARCA" },
                    { "DM", 100000515L, "DOMINICA" },
                    { "BT", 100000306L, "BHUTAN" },
                    { "BS", 100000505L, "BAHAMAS" },
                    { "BR", 100000605L, "BRASILE" },
                    { "BQ", 100000232L, "PAESI BASSI" },
                    { "AE", 100000322L, "EMIRATI ARABI UNITI" },
                    { "AF", 100000301L, "AFGHANISTAN" },
                    { "AG", 100000503L, "ANTIGUA E BARBUDA" },
                    { "AI", 100000402L, "REGNO UNITO" },
                    { "AL", 100000201L, "ALBANIA" },
                    { "AM", 100000358L, "ARMENIA" },
                    { "AO", 100000402L, "ANGOLA" },
                    { "AQ", 100000733L, "ARGENTINA" },
                    { "AR", 100000602L, "ARGENTINA" },
                    { "AS", 100000798L, "SAMOA" },
                    { "AT", 100000203L, "AUSTRIA" },
                    { "AU", 100000701L, "AUSTRALIA" },
                    { "AW", 100000358L, "PAESI BASSI" },
                    { "DO", 100000997L, "REPUBBLICA DOMINICANA" },
                    { "AX", 100000223L, "FINLANDIA" },
                    { "BA", 100000252L, "BOSNIA-ERZEGOVINA" },
                    { "BB", 100000506L, "BARBADOS" },
                    { "BD", 100000305L, "BANGLADESH" },
                    { "BE", 100000206L, "BELGIO" },
                    { "BF", 100000409L, "BURKINA-FASO" },
                    { "BG", 100000209L, "BULGARIA" },
                    { "BH", 100000304L, "BAHREIN" },
                    { "BI", 100000410L, "BURUNDI" },
                    { "BJ", 100000406L, "BENIN" },
                    { "BL", 100000797L, "FRANCIA" },
                    { "BM", 100000406L, "REGNO UNITO" },
                    { "BN", 100000309L, "BRUNEI" },
                    { "BO", 100000604L, "BOLIVIA" },
                    { "AZ", 100000359L, "AZERBAIGIAN" },
                    { "ZM", 100000464L, "ZAMBIA" },
                    { "DZ", 100000401L, "ALGERIA" },
                    { "EE", 100000247L, "ESTONIA" },
                    { "HN", 100000525L, "HONDURAS" },
                    { "HR", 100000250L, "CROAZIA" },
                    { "HT", 100000524L, "HAITI" },
                    { "HU", 100000244L, "UNGHERIA" },
                    { "ID", 100000331L, "INDONESIA" },
                    { "IE", 100000221L, "IRLANDA" },
                    { "IL", 100000334L, "ISRAELE" },
                    { "IM", 100000223L, "REGNO UNITO" },
                    { "IN", 100000330L, "INDIA" },
                    { "IO", 100000457L, "REGNO UNITO" },
                    { "IQ", 100000333L, "IRAQ" },
                    { "IR", 100000332L, "IRAN, REPUBBLICA ISLAMICA DEL" },
                    { "IS", 100000223L, "ISLANDA" },
                    { "IT", 100000100L, "ITALIA" },
                    { "JE", 100000223L, "JERSEY, ISOLE" },
                    { "JM", 100000518L, "GIAMAICA" },
                    { "JO", 100000327L, "GIORDANIA" },
                    { "JP", 100000326L, "GIAPPONE" },
                    { "KE", 100000428L, "KENIA" },
                    { "KG", 100000361L, "KIRGHIZISTAN" },
                    { "KH", 100000310L, "CAMBOGIA" },
                    { "KI", 100000708L, "KIRIBATI" },
                    { "KM", 100000417L, "COMORE" },
                    { "KN", 100000795L, "SAINT KITTS E NEVIS" },
                    { "KP", 100000319L, "REP.POPOLARE DEMOCRATICA DI COREA (COREA DEL NORD)" },
                    { "KR", 100000320L, "REPUBBLICA DI COREA (COREA DEL SUD)" },
                    { "KW", 100000335L, "KUWAIT" },
                    { "HM", 100000223L, "AUSTRALIA" },
                    { "HK", 110000005L, "REPUBBLICA POPOLARE CINESE" },
                    { "GY", 100000612L, "GUYANA" },
                    { "GW", 100000427L, "GUINEA BISSAU" },
                    { "EG", 100000419L, "EGITTO" },
                    { "EH", 100000533L, "MAROCCO" },
                    { "ER", 100000466L, "ERITREA" },
                    { "ES", 100000239L, "SPAGNA" },
                    { "ET", 100000420L, "ETIOPIA" },
                    { "FI", 100000214L, "FINLANDIA" },
                    { "FJ", 100000703L, "FIGI" },
                    { "FK", 100000223L, "REGNO UNITO" },
                    { "FM", 100000311L, "MICRONESIA" },
                    { "FO", 100000755L, "DANIMARCA" },
                    { "FR", 100000215L, "FRANCIA" },
                    { "GA", 100000421L, "GABON" },
                    { "GB", 100000219L, "REGNO UNITO" },
                    { "EC", 100000609L, "ECUADOR" },
                    { "GD", 100000519L, "GRENADA" },
                    { "GF", 100000612L, "GUYANA" },
                    { "GG", 100000761L, "REGNO UNITO" },
                    { "GH", 100000423L, "GHANA" },
                    { "GI", 100000326L, "REGNO UNITO" },
                    { "GL", 100000758L, "DANIMARCA" },
                    { "GM", 100000422L, "GAMBIA" },
                    { "GN", 100000425L, "GUINEA" },
                    { "GP", 100000759L, "FRANCIA" },
                    { "GQ", 100000427L, "GUINEA EQUATORIALE" },
                    { "GR", 100000220L, "GRECIA" },
                    { "GS", 100000360L, "REGNO UNITO" },
                    { "GT", 100000523L, "GUATEMALA" },
                    { "GU", 100000760L, "STATI UNITI D'AMERICA" },
                    { "GE", 100000360L, "GEORGIA" },
                    { "ZW", 100000465L, "ZIMBABWE" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoomAssignments_RoomId",
                table: "RoomAssignments",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_FloorId",
                table: "Rooms",
                column: "FloorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StructureId",
                table: "Users",
                column: "StructureId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorAssignments");

            migrationBuilder.DropTable(
                name: "Nations");

            migrationBuilder.DropTable(
                name: "RoomAssignments");

            migrationBuilder.DropTable(
                name: "UserRefreshTokens");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Structures");

            migrationBuilder.DropTable(
                name: "Floors");
        }
    }
}
