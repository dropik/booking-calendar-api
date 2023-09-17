using Microsoft.EntityFrameworkCore.Migrations;

namespace BookingCalendarApi.NETFramework.Migrations
{
    public partial class AddClearRefreshTokenEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
create event clear_refresh_tokens
on schedule every 1 day
starts current_timestamp
do
  delete from userrefreshtokens
  where ExpiresAt < now();");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("drop event if exists clear_refresh_tokens");
        }
    }
}
