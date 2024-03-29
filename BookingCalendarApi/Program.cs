using AlloggiatiService;

using BookingCalendarApi;
using BookingCalendarApi.Clients;
using BookingCalendarApi.Filters;
using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Common;
using BookingCalendarApi.Repository.NETCore;
using BookingCalendarApi.Services;

using C59Service;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using Serilog;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddSystemsManager(config =>
{
    string version = System.Environment.GetEnvironmentVariable("AWS_LAMBDA_FUNCTION_VERSION") ?? "";
    string stage = version.ToLower() == "$latest" ? "Dev" : "Prod";
    config.Path = $"/booking-calendar/{stage}/";
    config.ReloadAfter = TimeSpan.FromMinutes(5);
    config.Optional = false;
});

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

builder.Services.AddOptions();
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

builder.Services.AddSerilog();

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

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "CorsPolicy",
        policy =>
        {
            policy
                .WithOrigins(builder.Configuration.GetValue<string>("FrontendUrl"))
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
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

// setting this factory for specyfing to use EF Core 6
QueryWrapperFactory.Current = new EFCore6QueryWrapperFactory();

// scoped services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserClaimsProvider, UserClaimsProvider>();
builder.Services.AddScoped<IIperbooking, Iperbooking>();
builder.Services.AddScoped<IAlloggiatiServiceSession, AlloggiatiServiceSession>();
builder.Services.AddScoped<DataContext>();
builder.Services.AddScoped<IStructureService, StructureService>();

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

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.WriteTo.Console();
});

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseCors("CorsPolicy");

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
