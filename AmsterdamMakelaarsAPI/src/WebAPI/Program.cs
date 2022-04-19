using System.Reflection;
using AspNetCoreRateLimit;
using Infrastructure;
using Infrastructure.Configurations;
using MediatR;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using ILogger = Serilog.ILogger;

var builder = WebApplication.CreateBuilder(args);

#region Serilog

//Remove already registered providers
builder.Logging.ClearProviders();

ILogger logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
builder.Logging.AddSerilog(logger);
builder.Services.AddSingleton(logger);

#endregion

#region RateLimit

//loads configuration from appsettings
//builder.Services.AddOptions();
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    config.AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true);
});

builder.Services.Configure<HttpClients>(
    builder.Configuration.GetSection("HttpClients"));

// needed to store rate limit counters and ip rules
//TODO: Won't work with distributed systems!. Use Redis
builder.Services.AddMemoryCache();

//load general configuration from appsettings
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

// inject counter and rules stores
builder.Services.AddInMemoryRateLimiting();

// configuration (resolvers, counter key builders)
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddMediatR(Assembly.Load("Application"));

#endregion

var policy = HttpPolicyExtensions
    .HandleTransientHttpError() // HttpRequestException, 5XX and 408
    .OrResult(response => (int)response.StatusCode == 401) // RetryAfter
    .WaitAndRetryAsync(new TimeSpan[]
    {
        TimeSpan.FromMinutes(1)
    });

builder.Services.AddHttpClient("funda", client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("HttpClients:AmsterdamMakelaarHttpClient:BaseUri").Value);
}).AddPolicyHandler(policy);

builder.Services.AddScoped<IAmsterdamMakelaarsHttpClient, AmsterdamMakelaarsHttpClient>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

app.UseIpRateLimiting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCaching();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();