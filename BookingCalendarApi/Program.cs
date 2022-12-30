using AlloggiatiService;
using BookingCalendarApi;
using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Services;
using C59Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BookingCalendarContext>((options) =>
    options.UseMySql(
        builder.Configuration.GetSection("DB:ConnectionString").Value,
        new MySqlServerVersion(new Version(8, 0, 29))
    )
);

// scoped services
builder.Services.AddScoped<IIperbooking, Iperbooking>();
builder.Services.AddScoped<IAlloggiatiServiceSession, AlloggiatiServiceSession>();
builder.Services.AddScoped<DataContext>();

// transient services
builder.Services.AddTransient<IBookingComposer, BookingComposer>();
builder.Services.AddTransient<IBookingShortComposer, BookingShortComposer>();
builder.Services.AddTransient<IBookingsCachingSession, BookingsCachingSession>();
builder.Services.AddTransient<ITileComposer, TileComposer>();
builder.Services.AddTransient<IAssignedBookingComposer, AssignedBookingComposer>();
builder.Services.AddTransient<IStayComposer, StayComposer>();
builder.Services.AddTransient<IServiceSoap, ServiceSoapClient>();
builder.Services.AddTransient<ITrackedRecordSerializer, TrackedRecordSerializer>();
builder.Services.AddTransient<IAssignedBookingWithGuestsProvider, AssignedBookingWithGuestsProvider>();
builder.Services.AddTransient<EC59ServiceEndpoint, EC59ServiceEndpointClient>();
builder.Services.AddTransient<IC59ServiceSession, C59ServiceSession>();
builder.Services.AddTransient<IPlaceConverter, PlaceConverter>();
builder.Services.AddTransient<INationConverter, NationConverter>();
builder.Services.AddTransient<ITrackedRecordsComposer, TrackedRecordsComposer>();

#nullable disable
builder.Services.AddTransient<Func<string, string, IEnumerable<Reservation>, ICityTaxCalculator>>(
    serviceProvider =>
        (from, to, reservations) =>
            new CityTaxPeriodTrimmer(
                from, to,
                new CityTaxOver10Days(
                    new CityTaxGuestRegistriesFilter(
                        reservations,
                        new SimpleCityTaxCalculator()
                    )
                )
            )
);
builder.Services.AddTransient<Func<IEnumerable<RoomAssignment>, IAssignedBookingComposer>>(
    serviceProvider => (assignments) => new AssignedBookingComposer(serviceProvider.GetService<BookingCalendarContext>(), assignments));
builder.Services.AddTransient<Func<Reservation, ITileWithClientsComposer>>(
    serviceProvider => (reservation) => new TileWithClientsComposer(serviceProvider.GetService<BookingCalendarContext>(), reservation));
builder.Services.AddTransient<Func<IEnumerable<Reservation>, IBookingWithClientsComposer>>(
    serviceProvider => (reservations) => new BookingWithClientsComposer(
        serviceProvider.GetService<Func<Reservation, ITileWithClientsComposer>>(),
        reservations,
        serviceProvider.GetService<BookingCalendarContext>()
    ));
#nullable enable

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
