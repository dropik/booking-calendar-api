using AlloggiatiService;
using BookingCalendarApi;
using BookingCalendarApi.Filters;
using BookingCalendarApi.Services;
using C59Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add<ExceptionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

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
builder.Services.AddTransient<IBookingService, BookingService>();
builder.Services.AddTransient<ICityTaxService, CityTaxService>();
builder.Services.AddTransient<IClientsService, ClientsService>();
builder.Services.AddTransient<IColorAssignmentsService, ColorAssignmentsService>();
builder.Services.AddTransient<IAssignedBookingComposer, AssignedBookingComposer>();
builder.Services.AddTransient<IServiceSoap, ServiceSoapClient>();
builder.Services.AddTransient<IAssignedBookingWithGuestsProvider, AssignedBookingWithGuestsProvider>();
builder.Services.AddTransient<EC59ServiceEndpoint, EC59ServiceEndpointClient>();
builder.Services.AddTransient<IC59ServiceSession, C59ServiceSession>();
builder.Services.AddTransient<IPlaceConverter, PlaceConverter>();
builder.Services.AddTransient<INationConverter, NationConverter>();
builder.Services.AddTransient<ITrackedRecordsComposer, TrackedRecordsComposer>();

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
