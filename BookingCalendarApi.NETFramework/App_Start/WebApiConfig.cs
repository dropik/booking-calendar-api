using BookingCalendarApi.Controllers;
using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.NETFramework.AlloggiatiService;
using BookingCalendarApi.NETFramework.C59Service;
using BookingCalendarApi.NETFramework.Clients;
using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.NETFramework;
using BookingCalendarApi.Services;

using Microsoft.Extensions.DependencyInjection;

using Newtonsoft.Json.Serialization;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace BookingCalendarApi.NETFramework
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.EnableCors(new EnableCorsAttribute(origins: "http://localhost:3000", headers: "*", methods: "*"));

            // filters
            config.Filters.Add(new ExceptionFilter());

            // Servizi e configurazione dell'API Web
            var services = new ServiceCollection();

            services.AddOptions();
            services.Configure<JWT>(jwt =>
            {
                jwt.Key = ConfigurationManager.AppSettings["JWT_Key"];
                jwt.Issuer = ConfigurationManager.AppSettings["JWT_Issuer"];
                jwt.AccessTokenExpirationMinutes = int.Parse(ConfigurationManager.AppSettings["JWT_AccessTokenExpirationMinutes"]);
                jwt.RefreshTokenExpirationMinutes = int.Parse(ConfigurationManager.AppSettings["JWT_RefreshTokenExpirationMinutes"]);
            });
            services.Configure<Models.Iperbooking.Auth>(auth =>
            {
                auth.IdHotel = ConfigurationManager.AppSettings["Iperbooking_IdHotel"];
                auth.Username = ConfigurationManager.AppSettings["Iperbooking_Username"];
                auth.Password = ConfigurationManager.AppSettings["Iperbooking_Password"];
            });
            services.Configure<Models.AlloggiatiService.Credentials>(credentials =>
            {
                credentials.Utente = ConfigurationManager.AppSettings["AlloggiatiService_Utente"];
                credentials.Password = ConfigurationManager.AppSettings["AlloggiatiService_Password"];
                credentials.WsKey = ConfigurationManager.AppSettings["AlloggiatiService_WsKey"];
            });
            services.Configure<Models.C59Service.Credentials>(credentials =>
            {
                credentials.Username = ConfigurationManager.AppSettings["C59Service_Username"];
                credentials.Password = ConfigurationManager.AppSettings["C59Service_Password"];
                credentials.Struttura = long.Parse(ConfigurationManager.AppSettings["C59Service_Struttura"]);
            });

            services.AddDbContext<BookingCalendarContext>();

            // controllers
            services.AddTransient<AssignmentsController>();
            services.AddTransient<AuthController>();
            services.AddTransient<BookingsController>();
            services.AddTransient<CityTaxController>();
            services.AddTransient<ClientsController>();
            services.AddTransient<FloorsController>();
            services.AddTransient<IstatController>();
            services.AddTransient<PoliceController>();
            services.AddTransient<RoomsController>();
            services.AddTransient<UsersController>();

            // scoped services
            services.AddScoped(p => new User());
            services.AddScoped<IUserClaimsProvider, UserClaimsProvider>();
            services.AddScoped<IIperbooking, Iperbooking>();
            services.AddScoped<IAlloggiatiServiceSession, AlloggiatiServiceSession>();
            services.AddScoped<DataContext>();

            // transient services
            services.AddTransient(p => HttpContext.Current.User as ClaimsPrincipal);
            services.AddTransient<IRepository, Repository.NETFramework.Repository>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IBookingsService, BookingsService>();
            services.AddTransient<IAssignmentsService, AssignmentsService>();
            services.AddTransient<ICityTaxService, CityTaxService>();
            services.AddTransient<IClientsService, ClientsService>();
            services.AddTransient<IFloorsService, FloorsService>();
            services.AddTransient<IIstatService, IstatService>();
            services.AddTransient<IPoliceService, PoliceService>();
            services.AddTransient<IRoomsService, RoomsService>();
            services.AddTransient<IAssignedBookingComposer, AssignedBookingComposer>();
            services.AddTransient<IPoliceClient, PoliceClient>();
            services.AddTransient<ServiceSoap, ServiceSoapClient>();
            services.AddTransient<IAssignedBookingWithGuestsProvider, AssignedBookingWithGuestsProvider>();
            services.AddTransient<EC59ServiceEndpoint, EC59ServiceEndpointClient>();
            services.AddTransient<IC59Client, C59Client>();
            services.AddTransient<IPlaceConverter, PlaceConverter>();
            services.AddTransient<INationConverter, NationConverter>();
            services.AddTransient<ITrackedRecordsComposer, TrackedRecordsComposer>();

            config.DependencyResolver = new DependencyResolver(services.BuildServiceProvider());

            config.Formatters.Remove(config.Formatters.JsonFormatter);
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Add(new JsonFormatter());

            // Route dell'API Web
            config.MapHttpAttributeRoutes();
        }

        private class JsonFormatter : MediaTypeFormatter
        {
            private readonly JsonSerializerOptions _options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            };

            public JsonFormatter()
            {
                SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));
                SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            }

            public override bool CanReadType(Type type)
            {
                return true;
            }

            public override bool CanWriteType(Type type)
            {
                return true;
            }

            public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
            {
                return this;
            }

            public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
            {
                return await JsonSerializer.DeserializeAsync(readStream, type, _options);
            }

            public override async Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger, CancellationToken cancellationToken)
            {
                return await JsonSerializer.DeserializeAsync(readStream, type, _options, cancellationToken);
            }

            public override IRequiredMemberSelector RequiredMemberSelector { get => base.RequiredMemberSelector; set => base.RequiredMemberSelector = value; }

            public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
            {
                headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
            {
                await JsonSerializer.SerializeAsync(writeStream, value, type, _options);
            }

            public override async Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext, CancellationToken cancellationToken)
            {
                await JsonSerializer.SerializeAsync(writeStream, value, type, _options, cancellationToken);
            }
        }
    }
}
