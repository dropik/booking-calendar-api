using AlloggiatiService;

using BookingCalendarApi;
using BookingCalendarApi.Clients;
using BookingCalendarApi.Filters;
using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.NETCore;
using BookingCalendarApi.Services;

using C59Service;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<BookingCalendarApi.Models.Iperbooking.Auth>(builder.Configuration.GetSection("Iperbooking"));
builder.Services.Configure<BookingCalendarApi.Models.AlloggiatiService.Credentials>(builder.Configuration.GetSection("AlloggiatiService"));
builder.Services.Configure<BookingCalendarApi.Models.C59Service.Credentials>(builder.Configuration.GetSection("C59Service"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]);
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
            }
            return Task.CompletedTask;
        },
    };
});

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
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped(s => s.GetService<IHttpContextAccessor>()!.HttpContext!.User);
builder.Services.AddScoped<IIperbooking, Iperbooking>();
builder.Services.AddScoped<IAlloggiatiServiceSession, AlloggiatiServiceSession>();
builder.Services.AddScoped<DataContext>();

// transient services
builder.Services.AddTransient<IRepository, Repository>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IBookingsService, BookingsService>();
builder.Services.AddTransient<IAssignmentsService, AssignmentsService>();
builder.Services.AddTransient<ICityTaxService, CityTaxService>();
builder.Services.AddTransient<IClientsService, ClientsService>();
builder.Services.AddTransient<IFloorsService, FloorsService>();
builder.Services.AddTransient<IIstatService, IstatService>();
builder.Services.AddTransient<IPoliceService, PoliceService>();
builder.Services.AddTransient<IRoomsService, RoomsService>();
builder.Services.AddTransient<IAssignedBookingComposer, AssignedBookingComposer>();
builder.Services.AddTransient<IPoliceClient, PoliceClient>();
builder.Services.AddTransient<IServiceSoap, ServiceSoapClient>();
builder.Services.AddTransient<IAssignedBookingWithGuestsProvider, AssignedBookingWithGuestsProvider>();
builder.Services.AddTransient<EC59ServiceEndpoint, EC59ServiceEndpointClient>();
builder.Services.AddTransient<IC59Client, C59Client>();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
