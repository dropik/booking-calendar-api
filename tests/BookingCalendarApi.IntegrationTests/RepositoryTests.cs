using System.Security.Claims;

using BookingCalendarApi.IntegrationTests.Infrastructure;
using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Common;
using BookingCalendarApi.Repository.Extensions;
using BookingCalendarApi.Repository.NETCore;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.IntegrationTests
{
    /// <summary>The EF Core repository on its own: structure (tenant) separation and the query wrapper.</summary>
    public class RepositoryTests : ApiTest
    {
        public RepositoryTests(ApiFixture fixture) : base(fixture)
        {
            QueryWrapperFactory.Current = new EFCore6QueryWrapperFactory();
        }

        private (Repository.NETCore.Repository Repository, BookingCalendarContext Context) RepositoryOf(long? structureId)
        {
            var claims = structureId == null ? Array.Empty<Claim>() : new[] { new Claim(JWT.STRUCTURE_CLAIM, structureId.ToString()!) };
            var accessor = new FixedHttpContextAccessor(new DefaultHttpContext { User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test")) });
            var context = Fixture.CreateContext();
            return (new Repository.NETCore.Repository(context, accessor), context);
        }

        private async Task<(Structure Mine, Structure Theirs)> TwoStructuresWithFloorsAsync()
        {
            var mine = await Fixture.SeedStructureAsync("mine");
            var theirs = await Fixture.SeedStructureAsync("theirs");
            await using var context = Fixture.CreateContext();
            context.Floors.AddRange(new Floor { StructureId = mine.Id, Name = "Mine" }, new Floor { StructureId = theirs.Id, Name = "Theirs" });
            await context.SaveChangesAsync();
            return (mine, theirs);
        }

        [Fact]
        public async Task Queries_return_only_the_data_of_the_current_structure()
        {
            var (mine, _) = await TwoStructuresWithFloorsAsync();
            var (repository, _) = RepositoryOf(mine.Id);

            (await repository.Floors.ToListAsync()).Select(f => f.Name).Should().Equal("Mine");
        }

        [Fact]
        public async Task Without_structure_claim_no_structure_data_is_visible()
        {
            await TwoStructuresWithFloorsAsync();
            var (repository, _) = RepositoryOf(null);

            (await repository.Floors.ToListAsync()).Should().BeEmpty();
        }

        [Fact]
        public async Task Data_not_belonging_to_a_structure_is_not_filtered()
        {
            var (mine, _) = await TwoStructuresWithFloorsAsync();
            var (repository, _) = RepositoryOf(mine.Id);

            (await repository.Structures.ToListAsync()).Should().HaveCountGreaterThanOrEqualTo(3);
            (await repository.Nations.AnyAsync(n => n.Iso == "IT")).Should().BeTrue();
        }

        [Fact]
        public async Task Added_data_belongs_to_the_current_structure()
        {
            var (mine, theirs) = await TwoStructuresWithFloorsAsync();
            var (repository, _) = RepositoryOf(mine.Id);

            var floor = repository.Add(new Floor { StructureId = theirs.Id, Name = "Sneaky" });
            await repository.SaveChangesAsync();

            await using var context = Fixture.CreateContext();
            (await context.Floors.SingleAsync(f => f.Id == floor.Id)).StructureId.Should().Be(mine.Id);
        }

        [Fact]
        public async Task Updated_data_stays_in_the_current_structure()
        {
            var (mine, theirs) = await TwoStructuresWithFloorsAsync();
            var (repository, _) = RepositoryOf(mine.Id);
            long floorId;
            await using (var setup = Fixture.CreateContext())
            {
                floorId = (await setup.Floors.SingleAsync(f => f.StructureId == mine.Id)).Id;
            }

            var floor = repository.Update(new Floor { Id = floorId, StructureId = theirs.Id, Name = "Renamed" });
            await repository.SaveChangesAsync();

            await using var context = Fixture.CreateContext();
            var stored = await context.Floors.SingleAsync(f => f.Id == floor.Id);
            (stored.StructureId, stored.Name).Should().Be((mine.Id, "Renamed"));
        }

        [Fact]
        public async Task Removed_data_is_deleted()
        {
            var (mine, _) = await TwoStructuresWithFloorsAsync();
            var (repository, _) = RepositoryOf(mine.Id);

            repository.Remove(await repository.Floors.SingleAsync());
            await repository.SaveChangesAsync();

            await using var context = Fixture.CreateContext();
            (await context.Floors.Select(f => f.Name).ToListAsync()).Should().Equal("Theirs");
        }

        [Fact]
        public async Task Saving_detaches_every_entity()
        {
            var (mine, _) = await TwoStructuresWithFloorsAsync();
            var (repository, context) = RepositoryOf(mine.Id);
            var floor = await repository.Floors.SingleAsync();
            floor.Name = "Changed";

            await repository.SaveChangesAsync();

            context.ChangeTracker.Entries().Should().BeEmpty();
        }

        [Fact]
        public async Task Query_wrapper_single_and_any_without_predicate()
        {
            var (mine, _) = await TwoStructuresWithFloorsAsync();
            var (repository, _) = RepositoryOf(mine.Id);
            var (empty, _) = RepositoryOf(12345);

            (await repository.Floors.SingleAsync()).Name.Should().Be("Mine");
            (await repository.Floors.SingleOrDefaultAsync())!.Name.Should().Be("Mine");
            (await repository.Floors.AnyAsync()).Should().BeTrue();
            (await empty.Floors.SingleOrDefaultAsync()).Should().BeNull();
            (await empty.Floors.AnyAsync()).Should().BeFalse();

            repository.Add(new Floor { Name = "Second" });
            await repository.SaveChangesAsync();
            await repository.Floors.Invoking(floors => floors.SingleAsync()).Should().ThrowAsync<InvalidOperationException>();
            await repository.Floors.Invoking(floors => floors.SingleOrDefaultAsync()).Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task Query_wrapper_dictionary_and_include()
        {
            var (mine, _) = await TwoStructuresWithFloorsAsync();
            var (repository, _) = RepositoryOf(mine.Id);
            var floor = await repository.Floors.SingleAsync();
            repository.Add(new Room { FloorId = floor.Id, Number = "101", Type = "Doppia" });
            await repository.SaveChangesAsync();

            (await repository.Floors.ToDictionaryAsync(f => f.Name)).Keys.Should().Equal("Mine");
            (await RepositoryExtensions.Include(repository.Floors, f => f.Rooms).SingleAsync()).Rooms.Select(r => r.Number).Should().Equal("101");
        }

        /// <summary>
        /// Unlike <see cref="HttpContextAccessor"/>, which keeps the context in a static AsyncLocal,
        /// every instance has its own context.
        /// </summary>
        private class FixedHttpContextAccessor : IHttpContextAccessor
        {
            public FixedHttpContextAccessor(HttpContext context)
            {
                HttpContext = context;
            }

            public HttpContext? HttpContext { get; set; }
        }
    }
}
