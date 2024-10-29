using DeziBotDebugInterface.Api.Clients;
using DeziBotDebugInterface.Api.Endpoints.Common;
using DeziBotDebugInterface.Api.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

// Logging
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();
builder.Services.AddSerilog(Log.Logger);

// Services
builder.Services
    .AddEndpoints(typeof(Program))
    .AddScoped<HttpProblemDetailsService>()
    .AddScoped<IDezibotRepository, DezibotRepository>()
    .AddScoped<ICommandRepository, CommandRepository>()
    .AddScoped<ISensorRepository, SensorRepository>()
    .AddScoped<IDezibotClient, DezibotClient>();

var app = builder.Build();

// Swagger
app.UseSwagger();
app.UseSwaggerUI();
app.Map("/", () => Results.Redirect("/swagger/index.html"));

// Endpoints
app.MapEndpoints(app.MapGroup("/api"));

// Exception handling
app.MapErrorEndpoint();
app.UseExceptionHandler(ErrorEndpoint.ErrorRoute);

await app.RunAsync();
