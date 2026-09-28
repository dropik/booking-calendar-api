using BookingCalendarApi.IntegrationTests.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.IntegrationTests
{
    public class MigrationsTests : ApiTest
    {
        public MigrationsTests(ApiFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Migrations_are_all_applied()
        {
            await using var context = Fixture.CreateContext();

            (await context.Database.GetPendingMigrationsAsync()).Should().BeEmpty();
            (await context.Database.GetAppliedMigrationsAsync()).Should().NotBeEmpty();
        }

        [Fact]
        public async Task Migrations_seed_the_master_structure_and_admin()
        {
            await using var context = Fixture.CreateContext();

            var master = await context.Users.SingleAsync(u => u.Id == -1);
            master.Username.Should().Be(ApiFixture.MasterUsername);
            master.IsAdmin.Should().BeTrue();
            master.StructureId.Should().Be(-1);
            (await context.Structures.SingleAsync(s => s.Id == -1)).Name.Should().Be("Master Hotel");
        }

        [Fact]
        public async Task Migrations_seed_the_nations()
        {
            await using var context = Fixture.CreateContext();

            (await context.Nations.SingleAsync(n => n.Iso == "IT")).Code.Should().Be(100000100);
            (await context.Nations.SingleAsync(n => n.Iso == "PK")).Description.Should().NotBeEmpty();
            (await context.Nations.CountAsync()).Should().BeGreaterThan(200);
        }
    }
}
