using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class NationConfiguration : IEntityTypeConfiguration<Nation>
    {
        public void Configure(EntityTypeBuilder<Nation> builder)
        {
            builder.HasKey(n => n.Iso);

            builder.Property(n => n.Iso).HasMaxLength(PropertyDefaults.ISO_NATION_LENGTH);
            builder.Property(n => n.Code).IsRequired();
            builder.Property(n => n.Description).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);

            builder
                .HasData(
                    new Nation("AD", 100000202, "ANDORRA"),
                    new Nation("AE", 100000322, "EMIRATI ARABI UNITI"),
                    new Nation("AF", 100000301, "AFGHANISTAN"),
                    new Nation("AG", 100000503, "ANTIGUA E BARBUDA"),
                    new Nation("AI", 100000402, "REGNO UNITO"),                 // belongs to UK
                    new Nation("AL", 100000201, "ALBANIA"),
                    new Nation("AM", 100000358, "ARMENIA"),
                    new Nation("AO", 100000402, "ANGOLA"),
                    new Nation("AQ", 100000733, "ARGENTINA"),                   // that's Antarctica... so just whatever state close enough goes here
                    new Nation("AR", 100000602, "ARGENTINA"),
                    new Nation("AS", 100000798, "SAMOA"),
                    new Nation("AT", 100000203, "AUSTRIA"),
                    new Nation("AU", 100000701, "AUSTRALIA"),
                    new Nation("AW", 100000358, "PAESI BASSI"),                 // belongs to Netherlands
                    new Nation("AX", 100000223, "FINLANDIA"),                   // belongs to Finland
                    new Nation("AZ", 100000359, "AZERBAIGIAN"),
                    new Nation("BA", 100000252, "BOSNIA-ERZEGOVINA"),
                    new Nation("BB", 100000506, "BARBADOS"),
                    new Nation("BD", 100000305, "BANGLADESH"),
                    new Nation("BE", 100000206, "BELGIO"),
                    new Nation("BF", 100000409, "BURKINA-FASO"),
                    new Nation("BG", 100000209, "BULGARIA"),
                    new Nation("BH", 100000304, "BAHREIN"),
                    new Nation("BI", 100000410, "BURUNDI"),
                    new Nation("BJ", 100000406, "BENIN"),
                    new Nation("BL", 100000797, "FRANCIA"),                     // belongs to France
                    new Nation("BM", 100000406, "REGNO UNITO"),                 // belongs to UK
                    new Nation("BN", 100000309, "BRUNEI"),
                    new Nation("BO", 100000604, "BOLIVIA"),
                    new Nation("BQ", 100000232, "PAESI BASSI"),                 // belongs to Netherlands
                    new Nation("BR", 100000605, "BRASILE"),
                    new Nation("BS", 100000505, "BAHAMAS"),
                    new Nation("BT", 100000306, "BHUTAN"),
                    new Nation("BV", 100000223, "NORVEGIA"),                    // belongs to Norway
                    new Nation("BW", 100000408, "BOTSWANA"),
                    new Nation("BY", 100000256, "BIELORUSSIA"),
                    new Nation("BZ", 100000507, "BELIZE"),
                    new Nation("CA", 100000509, "CANADA"),
                    new Nation("CC", 100000223, "AUSTRALIA"),                   // belongs to Australia
                    new Nation("CD", 100000998, "REPUBBLICA DEMOCRATICA DEL CONGO (EX ZAIRE)"),
                    new Nation("CF", 100000257, "REPUBBLICA CENTRAFRICANA"),
                    new Nation("CG", 100000257, "CONGO"),
                    new Nation("CH", 100000241, "SVIZZERA"),
                    new Nation("CI", 100000404, "COSTA D'AVORIO"),
                    new Nation("CK", 100000223, "NUOVA ZELANDA"),               // belongs to New Zeland
                    new Nation("CL", 100000606, "CILE"),
                    new Nation("CM", 100000411, "CAMERUN"),
                    new Nation("CN", 100000606, "REPUBBLICA POPOLARE CINESE"),
                    new Nation("CO", 100000608, "COLOMBIA"),
                    new Nation("CR", 100000404, "COSTARICA"),
                    new Nation("CU", 100000514, "CUBA"),
                    new Nation("CV", 100000413, "CAPO VERDE"),
                    new Nation("CW", 100000514, "PAESI BASSI"),                 // belongs to Netherlands
                    new Nation("CX", 100000223, "AUSTRALIA"),                   // belongs to Australia
                    new Nation("CY", 100000315, "CIPRO"),
                    new Nation("CZ", 100000257, "REPUBBLICA CECA"),
                    new Nation("DE", 100000216, "GERMANIA"),
                    new Nation("DJ", 100000424, "GIBUTI"),
                    new Nation("DK", 100000212, "DANIMARCA"),
                    new Nation("DM", 100000515, "DOMINICA"),
                    new Nation("DO", 100000997, "REPUBBLICA DOMINICANA"),
                    new Nation("DZ", 100000401, "ALGERIA"),
                    new Nation("EC", 100000609, "ECUADOR"),
                    new Nation("EE", 100000247, "ESTONIA"),
                    new Nation("EG", 100000419, "EGITTO"),
                    new Nation("EH", 100000533, "MAROCCO"),                     // belongs to Marocco
                    new Nation("ER", 100000466, "ERITREA"),
                    new Nation("ES", 100000239, "SPAGNA"),
                    new Nation("ET", 100000420, "ETIOPIA"),
                    new Nation("FI", 100000214, "FINLANDIA"),
                    new Nation("FJ", 100000703, "FIGI"),
                    new Nation("FK", 100000223, "REGNO UNITO"),                 // belongs to UK
                    new Nation("FM", 100000311, "MICRONESIA"),
                    new Nation("FO", 100000755, "DANIMARCA"),                   // belongs to Danmark
                    new Nation("FR", 100000215, "FRANCIA"),
                    new Nation("GA", 100000421, "GABON"),
                    new Nation("GB", 100000219, "REGNO UNITO"),
                    new Nation("GD", 100000519, "GRENADA"),
                    new Nation("GE", 100000360, "GEORGIA"),
                    new Nation("GF", 100000612, "GUYANA"),
                    new Nation("GG", 100000761, "REGNO UNITO"),                 // belongs to UK
                    new Nation("GH", 100000423, "GHANA"),
                    new Nation("GI", 100000326, "REGNO UNITO"),                 // belongs to UK
                    new Nation("GL", 100000758, "DANIMARCA"),                   // belongs to Denmark
                    new Nation("GM", 100000422, "GAMBIA"),
                    new Nation("GN", 100000425, "GUINEA"),
                    new Nation("GP", 100000759, "FRANCIA"),                     // belongs to France
                    new Nation("GQ", 100000427, "GUINEA EQUATORIALE"),
                    new Nation("GR", 100000220, "GRECIA"),
                    new Nation("GS", 100000360, "REGNO UNITO"),                 // belongs to UK
                    new Nation("GT", 100000523, "GUATEMALA"),
                    new Nation("GU", 100000760, "STATI UNITI D'AMERICA"),       // belongs to USA
                    new Nation("GW", 100000427, "GUINEA BISSAU"),
                    new Nation("GY", 100000612, "GUYANA"),
                    new Nation("HK", 110000005, "REPUBBLICA POPOLARE CINESE"),  // belongs to China
                    new Nation("HM", 100000223, "AUSTRALIA"),                   // belongs to Australia
                    new Nation("HN", 100000525, "HONDURAS"),
                    new Nation("HR", 100000250, "CROAZIA"),
                    new Nation("HT", 100000524, "HAITI"),
                    new Nation("HU", 100000244, "UNGHERIA"),
                    new Nation("ID", 100000331, "INDONESIA"),
                    new Nation("IE", 100000221, "IRLANDA"),
                    new Nation("IL", 100000334, "ISRAELE"),
                    new Nation("IM", 100000223, "REGNO UNITO"),                 // not sure who belongs to, but government relates to UK
                    new Nation("IN", 100000330, "INDIA"),
                    new Nation("IO", 100000457, "REGNO UNITO"),                 // belongs to UK
                    new Nation("IQ", 100000333, "IRAQ"),
                    new Nation("IR", 100000332, "IRAN, REPUBBLICA ISLAMICA DEL"),
                    new Nation("IS", 100000223, "ISLANDA"),
                    new Nation("IT", 100000100, "ITALIA"),
                    new Nation("JE", 100000223, "JERSEY, ISOLE"),
                    new Nation("JM", 100000518, "GIAMAICA"),
                    new Nation("JO", 100000327, "GIORDANIA"),
                    new Nation("JP", 100000326, "GIAPPONE"),
                    new Nation("KE", 100000428, "KENIA"),
                    new Nation("KG", 100000361, "KIRGHIZISTAN"),
                    new Nation("KH", 100000310, "CAMBOGIA"),
                    new Nation("KI", 100000708, "KIRIBATI"),
                    new Nation("KM", 100000417, "COMORE"),
                    new Nation("KN", 100000795, "SAINT KITTS E NEVIS"),
                    new Nation("KP", 100000319, "REP.POPOLARE DEMOCRATICA DI COREA (COREA DEL NORD)"),
                    new Nation("KR", 100000320, "REPUBBLICA DI COREA (COREA DEL SUD)"),
                    new Nation("KW", 100000335, "KUWAIT"),
                    new Nation("KY", 100000223, "REGNO UNITO"),                 // belongs to UK
                    new Nation("KZ", 100000356, "KAZAKISTAN"),
                    new Nation("LA", 100000336, "LAOS"),
                    new Nation("LB", 100000337, "LIBANO"),
                    new Nation("LC", 100000532, "SAINT LUCIA"),
                    new Nation("LI", 100000225, "LIECHTENSTEIN"),
                    new Nation("LK", 100000239, "SRI LANKA (CEYLON)"),
                    new Nation("LR", 100000430, "LIBERIA"),
                    new Nation("LS", 100000429, "LESOTHO"),
                    new Nation("LT", 100000249, "LITUANIA"),
                    new Nation("LU", 100000226, "LUSSEMBURGO"),
                    new Nation("LV", 100000248, "LETTONIA"),
                    new Nation("LY", 100000431, "LIBIA"),
                    new Nation("MA", 100000436, "MAROCCO"),
                    new Nation("MC", 100000234, "MONACO"),
                    new Nation("MD", 100000254, "MOLDOVA"),
                    new Nation("ME", 100001001, "MONTENEGRO"),
                    new Nation("MF", 100000797, "FRANCIA"),                     // belongs to France
                    new Nation("MG", 100000432, "MADAGASCAR"),
                    new Nation("MH", 100000223, "MARSHALL"),
                    new Nation("MK", 100000253, "MACEDONIA (EX REPUBBLICA JUGOSLAVA)"),
                    new Nation("ML", 100000435, "MALI"),
                    new Nation("MM", 100000256, "MYANMAR"),
                    new Nation("MN", 100000341, "MONGOLIA"),
                    new Nation("MO", 110000003, "REPUBBLICA POPOLARE CINESE"),
                    new Nation("MP", 100000223, "STATI UNITI D'AMERICA"),       // belongs to USA
                    new Nation("MQ", 100000773, "FRANCIA"),                     // belongs to France
                    new Nation("MR", 100000437, "MAURITANIA"),
                    new Nation("MS", 100000777, "REGNO UNITO"),                 // belongs to UK
                    new Nation("MT", 100000227, "MALTA"),
                    new Nation("MU", 100000437, "MAURITIUS"),
                    new Nation("MV", 100000339, "MALDIVE"),
                    new Nation("MW", 100000434, "MALAWI"),
                    new Nation("MX", 100000527, "MESSICO"),
                    new Nation("MY", 100000767, "MALAYSIA"),
                    new Nation("MZ", 100000440, "MOZAMBICO"),
                    new Nation("NA", 100000441, "NAMIBIA"),
                    new Nation("NC", 100000780, "FRANCIA"),                     // belongs to France
                    new Nation("NE", 100000442, "NIGER"),
                    new Nation("NF", 100000223, "AUSTRALIA"),                   // belongs to Australia
                    new Nation("NG", 100000443, "NIGERIA"),
                    new Nation("NI", 100000529, "NICARAGUA"),
                    new Nation("NL", 100000232, "PAESI BASSI"),
                    new Nation("NO", 100000231, "NORVEGIA"),
                    new Nation("NP", 100000342, "NEPAL"),
                    new Nation("NR", 100000715, "NAURU"),
                    new Nation("NU", 100000443, "NUOVA ZELANDA"),               // belongs to New Zeland
                    new Nation("NZ", 100000719, "NUOVA ZELANDA"),
                    new Nation("OM", 100000343, "OMAN"),
                    new Nation("PA", 100000530, "PANAMA"),
                    new Nation("PE", 100000615, "PERU'"),
                    new Nation("PF", 100000787, "FRANCIA"),                     // belongs to France
                    new Nation("PG", 100000530, "PAPUA NUOVA GUINEA"),
                    new Nation("PH", 100000323, "FILIPPINE"),
                    new Nation("PK", 100000344, "PAKISTAN"),
                    new Nation("PL", 100000233, "POLONIA"),
                    new Nation("PM", 100000797, "FRANCIA"),                     // belongs to France
                    new Nation("PN", 100000223, "REGNO UNITO"),                 // belongs to UK
                    new Nation("PR", 100000233, "STATI UNITI D'AMERICA"),       // belongs to USA
                    new Nation("PS", 100000536, "PAKISTAN"),
                    new Nation("PT", 100000234, "PORTOGALLO"),
                    new Nation("PW", 100000344, "PALAU"),
                    new Nation("PY", 100000614, "PARAGUAY"),
                    new Nation("QA", 100000345, "QATAR"),
                    new Nation("RE", 100000765, "FRANCIA"),                     // belongs to France
                    new Nation("RO", 100000235, "ROMANIA"),
                    new Nation("RS", 100001000, "SERBIA REPUBBLICA DI"),
                    new Nation("RU", 100000245, "FEDERAZIONE RUSSA"),
                    new Nation("RW", 100000446, "RUANDA"),
                    new Nation("SA", 100000302, "ARABIA SAUDITA"),
                    new Nation("SB", 100000223, "SALOMONE"),
                    new Nation("SC", 100001000, "SEYCHELLES"),
                    new Nation("SD", 100000455, "SUDAN"),
                    new Nation("SE", 100000240, "SVEZIA"),
                    new Nation("SG", 100000346, "SINGAPORE"),
                    new Nation("SH", 100000799, "REGNO UNITO"),                 // belongs to UK
                    new Nation("SI", 100000251, "SLOVENIA"),
                    new Nation("SJ", 100000616, "NORVEGIA"),                    // belongs to Norway
                    new Nation("SK", 100000348, "REPUBBLICA SLOVACCA"),
                    new Nation("SL", 100000451, "SIERRA LEONE"),
                    new Nation("SM", 100000236, "SAN MARINO"),
                    new Nation("SN", 100000450, "SENEGAL"),
                    new Nation("SO", 100000453, "SOMALIA"),
                    new Nation("SR", 100000616, "SURINAME"),
                    new Nation("SS", 100000455, "SUD SUDAN, REPUBBLICA DEL"),
                    new Nation("ST", 100000448, "SAO TOME' E PRINCIPE"),
                    new Nation("SV", 100000517, "EL SALVADOR"),
                    new Nation("SX", 100000346, "PAESI BASSI"),                 // belongs to Netherlands
                    new Nation("SY", 100000348, "SIRIA"),
                    new Nation("SZ", 100000456, "SWAZILAND"),
                    new Nation("TC", 100000223, "REGNO UNITO"),                 // belongs to UK
                    new Nation("TD", 100000415, "CIAD"),
                    new Nation("TF", 100000457, "FRANCIA"),                     // belongs to France
                    new Nation("TG", 100000458, "TOGO"),
                    new Nation("TH", 100000349, "THAILANDIA"),
                    new Nation("TJ", 100000362, "TAGIKISTAN"),
                    new Nation("TK", 100000806, "NUOVA ZELANDA"),               // belongs to New Zeland
                    new Nation("TL", 100000805, "TIMOR ORIENTALE"),
                    new Nation("TM", 100000364, "TURKMENISTAN"),
                    new Nation("TN", 100000460, "TUNISIA"),
                    new Nation("TO", 100000730, "TONGA"),
                    new Nation("TR", 100000351, "TURCHIA"),
                    new Nation("TT", 100000617, "TRINIDAD E TOBAGO"),
                    new Nation("TV", 100000731, "TUVALU"),
                    new Nation("TW", 100000998, "TAIWAN"),
                    new Nation("TZ", 100000457, "TANZANIA"),
                    new Nation("UA", 100000243, "UCRAINA"),
                    new Nation("UG", 100000461, "UGANDA"),
                    new Nation("UK", 100000219, "REGNO UNITO"),
                    new Nation("UM", 100000223, "STATI UNITI D'AMERICA"),       // belongs to USA
                    new Nation("US", 100000536, "STATI UNITI D'AMERICA"),
                    new Nation("UY", 100000618, "URUGUAY"),
                    new Nation("UZ", 100000357, "UZBEKISTAN"),
                    new Nation("VA", 100000246, "SANTA SEDE"),
                    new Nation("VC", 100000797, "S. VINCENT E GRENADINE"),
                    new Nation("VE", 100000619, "VENEZUELA"),
                    new Nation("VG", 100000764, "REGNO UNITO"),                 // belongs to UK
                    new Nation("VI", 100000764, "STATI UNITI D'AMERICA"),       // belongs to USA
                    new Nation("VN", 100000353, "VIETNAM"),
                    new Nation("VU", 100000732, "VANUATU"),
                    new Nation("WF", 100000815, "FRANCIA"),                     // belongs to France
                    new Nation("WS", 100000727, "SAMOA"),
                    new Nation("YE", 100000354, "YEMEN"),
                    new Nation("YT", 100000774, "FRANCIA"),                     // belongs to France
                    new Nation("ZA", 100000467, "SUD AFRICA"),
                    new Nation("ZM", 100000464, "ZAMBIA"),
                    new Nation("ZW", 100000465, "ZIMBABWE"));
        }
    }
}
