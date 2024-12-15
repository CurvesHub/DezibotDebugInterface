using System.Diagnostics.CodeAnalysis;

using DezibotDebugInterface.Api.DataAccess;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DezibotDebugInterface.Api.Tests.TestCommon;

/// <summary>
/// A web application factory for the Dezibot API.
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by xUnit.")]
public class DezibotApiFactory(string testName) : WebApplicationFactory<Program>
{
    /// <summary>
    /// Resolves a <see cref="DezibotDbContext"/> from the services.
    /// </summary>
    /// <returns>A <see cref="DezibotDbContext"/>.</returns>
    public DezibotDbContext ResolveDbContext()
    {
        return Services.CreateAsyncScope().ServiceProvider.GetRequiredService<DezibotDbContext>();
    }
    
    /// <summary>
    /// Creates the database for the tests.
    /// </summary>
    public async Task CreateDatabaseAsync()
    {
        await using var dbContext = ResolveDbContext();
        await dbContext.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Deletes the database for the tests.
    /// </summary>
    public async Task DeleteDatabaseAsync()
    {
        await using var dbContext = ResolveDbContext();
        await dbContext.Database.EnsureDeletedAsync();
    }

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        if (testName is null)
        {
            throw new InvalidOperationException("Test name must be set before configuring the web host.");
        }
        
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DezibotDbContext>();
            services.RemoveAll<DbContextOptions<DezibotDbContext>>();

            services.AddDbContext<DezibotDbContext>(options =>
            {
                options.UseSqlite($"Data Source=TestDezibot_{testName}.db",
                    sqliteOptions => sqliteOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
            });
        });

        base.ConfigureWebHost(builder);
    }
}