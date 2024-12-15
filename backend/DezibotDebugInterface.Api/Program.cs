using DezibotDebugInterface.Api;
using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.Development;

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

namespace DezibotDebugInterface.Api
{
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>
    /// We need to define the `Program` class as partial here to be able to use it in the end-to-end tests with the WebApplicationFactory{TEntryPoint}
    /// </summary>
    public partial class Program;
}