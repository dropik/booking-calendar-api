using BookingCalendarApi;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Services;
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

builder.Services.AddSingleton<IIperbooking, Iperbooking>();
builder.Services.AddTransient<IBookingsProvider, BookingsProvider>();
builder.Services.AddTransient<IBookingComposer, BookingComposer>();
builder.Services.AddTransient<IBookingShortComposer, BookingShortComposer>();
builder.Services.AddTransient<BookingCalendarApi.Services.ISession, Session>();
builder.Services.AddTransient<ITileComposer, TileComposer>();
builder.Services.AddTransient<IStayComposer, StayComposer>();

#nullable disable
builder.Services.AddTransient<Func<BookingCalendarApi.Services.ISession>>(
    serviceProvider => () => serviceProvider.GetService<BookingCalendarApi.Services.ISession>());
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
