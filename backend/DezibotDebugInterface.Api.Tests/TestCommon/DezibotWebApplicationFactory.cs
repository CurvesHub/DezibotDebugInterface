using System.Diagnostics.CodeAnalysis;

using DezibotDebugInterface.Api.DataAccess;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DezibotDebugInterface.Api.Tests.TestCommon;

/// <summary>
/// A web application factory for the Dezibot API.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by xUnit.")]
public class DezibotWebApplicationFactory(string testName) : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DezibotDbContext>();
            services.RemoveAll<DbContextOptions<DezibotDbContext>>();

            services.AddDbContext<DezibotDbContext>(options =>
            {
                options.UseSqlite($"Data Source=TestDezibot_{testName}.db");
            });
        });

        base.ConfigureWebHost(builder);
    }

    public async Task CreateDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DeleteDatabaseAsync()
    {
        await using var scope = Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        await dbContext.Database.EnsureDeletedAsync();
    }

    public async Task InitializeAsync()
    {
        await CreateDatabaseAsync();
    }

    public new async Task DisposeAsync()
    {
        await DeleteDatabaseAsync();
    }
}