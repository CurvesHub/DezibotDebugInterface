using DezibotDebugInterface.Api;
using DezibotDebugInterface.Api.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProjectDependencies(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();
app.MapProjectEndpoints();

app.UseExceptionHandler("/error");
app.MapErrorEndpoint();

await MigrationService.MigrateDatabaseAsync(app.Services);

await app.RunAsync();

namespace DezibotDebugInterface.Api
{
    // ReSharper disable once PartialTypeWithSinglePart
    // ReSharper disable once ClassNeverInstantiated.Global
    /// <summary>
    /// We need to define the `Program` class as partial here to be able to use it in the end-to-end tests with the WebApplicationFactory{TEntryPoint}
    /// </summary>
    public partial class Program;
}