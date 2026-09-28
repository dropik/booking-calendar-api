using AlloggiatiService;

using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;

using C59Service;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using MySqlConnector;

using NSubstitute.ClearExtensions;

using Testcontainers.MySql;

using ApiEnvironment = BookingCalendarApi.Models.Configurations.Environment;

namespace BookingCalendarApi.IntegrationTests.Infrastructure
{
    /// <summary>
    /// One MySQL 8 database (migrated with the real EF Core migrations) and one API host per test run.
    /// External services are replaced: the Alloggiati and C59 SOAP clients by substitutes, Iperbooking by <see cref="FakeIperbooking"/>.
    /// Set TEST_MYSQL_CONNECTION_STRING to use an existing MySQL server instead of a Testcontainers one.
    /// </summary>
    public class ApiFixture : IAsyncLifetime
    {
        public const string MasterUsername = "master@bookingcalendar.com";
        public const string MasterPassword = "master-password";
        public const string JwtKey = "integration-tests-signing-key-long-enough-for-hmac-sha256";
        public const string JwtIssuer = "integration-tests";

        private MySqlContainer? _container;
        private readonly Dictionary<ApiEnvironment, WebApplicationFactory<Program>> _factories = new();

        public string ConnectionString { get; private set; } = "";
        public IServiceSoap PoliceService { get; } = Substitute.For<IServiceSoap>();
        public EC59ServiceEndpoint IstatService { get; } = Substitute.For<EC59ServiceEndpoint>();
        public FakeIperbooking Iperbooking { get; } = new();

        public async Task InitializeAsync()
        {
            var external = System.Environment.GetEnvironmentVariable("TEST_MYSQL_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(external))
            {
                var builder = new MySqlConnectionStringBuilder(external) { Database = $"booking_calendar_tests_{Guid.NewGuid():N}" };
                ConnectionString = builder.ConnectionString;
            }
            else
            {
                _container = new MySqlBuilder().WithImage("mysql:8.0").WithUsername("root").WithPassword("root").Build();
                await _container.StartAsync();
                ConnectionString = new MySqlConnectionStringBuilder(_container.GetConnectionString()) { Database = "booking_calendar_tests" }.ConnectionString;
            }

            // read by the AddMasterUser migration
            System.Environment.SetEnvironmentVariable("MasterPassword", MasterPassword);
            await using (var context = CreateContext())
            {
                await context.Database.MigrateAsync();
            }

            System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        }

        public async Task DisposeAsync()
        {
            foreach (var factory in _factories.Values)
            {
                await factory.DisposeAsync();
            }
            if (_container != null)
            {
                await _container.DisposeAsync();
            }
            else
            {
                await using var context = CreateContext();
                await context.Database.EnsureDeletedAsync();
            }
        }

        public BookingCalendarContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<BookingCalendarContext>()
                .UseMySql(ConnectionString, new MySqlServerVersion(new Version(8, 0, 29)))
                .Options;
            return new BookingCalendarContext(options);
        }

        public WebApplicationFactory<Program> Factory(ApiEnvironment environment = ApiEnvironment.Production)
        {
            if (!_factories.TryGetValue(environment, out var factory))
            {
                factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
                {
                    builder.UseEnvironment("Testing");
                    builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["DB:ConnectionString"] = ConnectionString,
                        ["JWT:Key"] = JwtKey,
                        ["JWT:Issuer"] = JwtIssuer,
                        ["FrontendUrl"] = "http://frontend.test",
                        ["ApiSettings:Environment"] = environment.ToString(),
                        ["Iperbooking:BaseUrl"] = "https://iperbooking.test/v1/",
                    }));
                    builder.ConfigureTestServices(services =>
                    {
                        services.RemoveAll<IServiceSoap>();
                        services.AddSingleton(PoliceService);
                        services.RemoveAll<EC59ServiceEndpoint>();
                        services.AddSingleton(IstatService);
                        services.AddHttpClient<IIperbooking, Iperbooking>()
                            .ConfigurePrimaryHttpMessageHandler(() => Iperbooking)
                            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
                    });
                });
                _factories[environment] = factory;
            }
            return factory;
        }

        /// <summary>Removes everything but the data seeded by the migrations and resets the fakes.</summary>
        public async Task ResetAsync()
        {
            Iperbooking.Reset();
            PoliceService.ClearSubstitute();
            IstatService.ClearSubstitute();

            await using var context = CreateContext();
            await context.Database.ExecuteSqlRawAsync(@"
                DELETE FROM RoomAssignments;
                DELETE FROM ColorAssignments;
                DELETE FROM Rooms;
                DELETE FROM Floors;
                DELETE FROM UserRefreshTokens;
                DELETE FROM Users WHERE Id <> -1;
                DELETE FROM Structures WHERE Id <> -1;");
        }

        public async Task<Structure> SeedStructureAsync(string name, Action<Structure>? configure = null)
        {
            var structure = new Structure
            {
                Name = name,
                IperbookingHotel = "4321",
                IperbookingUsername = $"{name}-ib-user",
                IperbookingPassword = $"{name}-ib-pwd",
                ASUtente = $"{name}-as-user",
                ASPassword = $"{name}-as-pwd",
                ASWsKey = $"{name}-as-key",
                C59Username = $"{name}-c59-user",
                C59Password = $"{name}-c59-pwd",
                C59Struttura = 555,
            };
            configure?.Invoke(structure);
            await using var context = CreateContext();
            context.Structures.Add(structure);
            await context.SaveChangesAsync();
            return structure;
        }

        /// <summary>Creates a floor with one room and assigns the given tiles to that room.</summary>
        public async Task<Room> SeedRoomAsync(long structureId, params string[] assignedTiles)
        {
            await using var context = CreateContext();
            var floor = new Floor { StructureId = structureId, Name = "Ground" };
            context.Floors.Add(floor);
            await context.SaveChangesAsync();
            var room = new Room { StructureId = structureId, FloorId = floor.Id, Number = "1", Type = "Doppia" };
            context.Rooms.Add(room);
            await context.SaveChangesAsync();
            context.RoomAssignments.AddRange(assignedTiles.Select(tile => new RoomAssignment { Id = tile, RoomId = room.Id, StructureId = structureId }));
            await context.SaveChangesAsync();
            return room;
        }

        public async Task<User> SeedUserAsync(long structureId, string username, string password, bool isAdmin = false)
        {
            var user = new User
            {
                StructureId = structureId,
                Username = username,
                PasswordHash = new PasswordHasher<string>().HashPassword(username, password),
                VisibleName = username.Split('@')[0],
                IsAdmin = isAdmin,
            };
            await using var context = CreateContext();
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }
    }

    [CollectionDefinition(Name)]
    public class ApiCollection : ICollectionFixture<ApiFixture>
    {
        public const string Name = "API";
    }
}
