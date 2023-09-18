using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.Iperbooking;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Extensions;

using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class StructureService : IStructureService
    {
        private readonly IUserClaimsProvider _claimsProvider;
        private readonly IRepository _repository;

        private Structure CurrentStructure { get; set; }

        public StructureService(IUserClaimsProvider user, IRepository repository)
        {
            _claimsProvider = user;
            _repository = repository;
        }

        public async Task<APIKeysResponse> GetApiKeys()
        {
            var iperbooking = await GetIperbookingCredentials();
            var police = await GetAlloggiatiServiceCredentials();
            var istat = await GetC59Credentials();

            return new APIKeysResponse
            {
                IperbookingHotel = iperbooking.IdHotel,
                IperbookingUsername = iperbooking.Username,
                IperbookingPassword = iperbooking.Password,

                ASUtente = police.Utente,
                ASPassword = police.Password,
                ASWsKey = police.WsKey,

                C59Struttura = istat.Struttura,
                C59Username = istat.Username,
                C59Password = istat.Password,
            };
        }

        public async Task UpdateApiKeys(UpdateAPIKeysRequest request)
        {
            await LoadCurrentStructure();

            CurrentStructure.IperbookingHotel = request.IperbookingHotel;
            CurrentStructure.IperbookingUsername = request.IperbookingUsername;
            CurrentStructure.IperbookingPassword = request.IperbookingPassword;

            CurrentStructure.ASUtente = request.ASUtente;
            CurrentStructure.ASPassword = request.ASPassword;
            CurrentStructure.ASWsKey = request.ASWsKey;

            CurrentStructure.C59Struttura = request.C59Struttura;
            CurrentStructure.C59Username = request.C59Username;
            CurrentStructure.C59Password = request.C59Password;

            _repository.Update(CurrentStructure);
            await _repository.SaveChangesAsync();
        }

        public async Task<Auth> GetIperbookingCredentials()
        {
            await LoadCurrentStructure();
            return new Auth()
            {
                IdHotel = CurrentStructure.IperbookingHotel,
                Username = CurrentStructure.IperbookingUsername,
                Password = CurrentStructure.IperbookingPassword,
            };
        }

        public async Task<Models.AlloggiatiService.Credentials> GetAlloggiatiServiceCredentials()
        {
            await LoadCurrentStructure();
            return new Models.AlloggiatiService.Credentials()
            {
                Utente = CurrentStructure.ASUtente,
                Password = CurrentStructure.ASPassword,
                WsKey = CurrentStructure.ASWsKey,
            };
        }

        public async Task<Models.C59Service.Credentials> GetC59Credentials()
        {
            await LoadCurrentStructure();
            return new Models.C59Service.Credentials()
            {
                Struttura = CurrentStructure.C59Struttura,
                Username = CurrentStructure.C59Username,
                Password = CurrentStructure.C59Password,
            };
        }

        private async Task LoadCurrentStructure()
        {
            if (CurrentStructure != null) return;

            var structureId = long.Parse(_claimsProvider.User.Claims.First(c => c.Type == JWT.STRUCTURE_CLAIM).Value);
            CurrentStructure = await _repository.Structures.SingleAsync(s => s.Id == structureId);
        }
    }
}
