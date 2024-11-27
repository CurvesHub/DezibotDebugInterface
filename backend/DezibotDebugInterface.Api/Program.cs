using DezibotDebugInterface.Api;
using DezibotDebugInterface.Api.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProjectDependencies(builder.Configuration);

var app = builder.Build();

app.MapProjectEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapDevelopmentEndpoints();
}

app.UseExceptionHandler("/error");
app.MapErrorEndpoint();

await MigrationService.MigrateDatabaseAsync(app.Services);

app.Run();

// We need to define the `Program` class as partial here to be able to use it in the end-to-end tests with the WebApplicationFactory<Program> class.
namespace DezibotDebugInterface.Api
{
    public partial class Program;
}