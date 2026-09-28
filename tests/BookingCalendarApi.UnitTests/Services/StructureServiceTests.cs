using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

namespace BookingCalendarApi.UnitTests.Services
{
    public class StructureServiceTests
    {
        private readonly InMemoryRepository _repository = new();
        private readonly StructureService _service;

        public StructureServiceTests()
        {
            _repository.Seed(new Structure { Id = 1, Name = "Other", IperbookingHotel = "x" });
            _repository.Seed(new Structure
            {
                Id = 2,
                Name = "Mine",
                IperbookingHotel = "123",
                IperbookingUsername = "ib-user",
                IperbookingPassword = "ib-pwd",
                ASUtente = "as-user",
                ASPassword = "as-pwd",
                ASWsKey = "as-key",
                C59Username = "c59-user",
                C59Password = "c59-pwd",
                C59Struttura = 555,
            });
            _service = new StructureService(TestPrincipals.Provider("mario@hotel.it", 2), _repository);
        }

        [Fact]
        public async Task GetApiKeys_returns_the_keys_of_the_current_structure()
        {
            var keys = await _service.GetApiKeys();

            keys.IperbookingHotel.Should().Be("123");
            keys.IperbookingUsername.Should().Be("ib-user");
            keys.IperbookingPassword.Should().Be("ib-pwd");
            keys.ASUtente.Should().Be("as-user");
            keys.ASPassword.Should().Be("as-pwd");
            keys.ASWsKey.Should().Be("as-key");
            keys.C59Username.Should().Be("c59-user");
            keys.C59Password.Should().Be("c59-pwd");
            keys.C59Struttura.Should().Be(555);
        }

        [Fact]
        public async Task UpdateApiKeys_stores_every_key_on_the_current_structure()
        {
            await _service.UpdateApiKeys(new UpdateAPIKeysRequest
            {
                IperbookingHotel = "h2",
                IperbookingUsername = "u2",
                IperbookingPassword = "p2",
                ASUtente = "au2",
                ASPassword = "ap2",
                ASWsKey = "ak2",
                C59Username = "cu2",
                C59Password = "cp2",
                C59Struttura = 777,
            });

            var stored = _repository.Stored<Structure>().Single(s => s.Id == 2);
            stored.Name.Should().Be("Mine");
            stored.IperbookingHotel.Should().Be("h2");
            stored.IperbookingUsername.Should().Be("u2");
            stored.IperbookingPassword.Should().Be("p2");
            stored.ASUtente.Should().Be("au2");
            stored.ASPassword.Should().Be("ap2");
            stored.ASWsKey.Should().Be("ak2");
            stored.C59Username.Should().Be("cu2");
            stored.C59Password.Should().Be("cp2");
            stored.C59Struttura.Should().Be(777);
            _repository.Stored<Structure>().Single(s => s.Id == 1).IperbookingHotel.Should().Be("x");
        }

        [Fact]
        public async Task Credentials_are_mapped_for_every_service()
        {
            var iperbooking = await _service.GetIperbookingCredentials();
            var police = await _service.GetAlloggiatiServiceCredentials();
            var istat = await _service.GetC59Credentials();

            (iperbooking.IdHotel, iperbooking.Username, iperbooking.Password).Should().Be(("123", "ib-user", "ib-pwd"));
            (police.Utente, police.Password, police.WsKey).Should().Be(("as-user", "as-pwd", "as-key"));
            (istat.Struttura, istat.Username, istat.Password).Should().Be((555L, "c59-user", "c59-pwd"));
        }

        [Fact]
        public async Task Structure_is_loaded_once_per_service_instance()
        {
            await _service.GetIperbookingCredentials();

            var structure = _repository.Structures.Single(s => s.Id == 2);
            _repository.Remove(structure);
            await _repository.SaveChangesAsync();

            (await _service.GetC59Credentials()).Username.Should().Be("c59-user");
        }
    }
}
