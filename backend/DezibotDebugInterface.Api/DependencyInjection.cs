using DezibotDebugInterface.Api.DataAccess;

using Microsoft.EntityFrameworkCore;

using Serilog;

namespace DezibotDebugInterface.Api;

/// <summary>
/// Provides extension methods for configuring project dependencies.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the project dependencies to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    public static void AddProjectDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        services
            .AddSerilog(Log.Logger)
            .AddSqliteDatabase(configuration)
            .AddOpenApi()
            .AddSignalR();
    }
    
    private static IServiceCollection AddSqliteDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DezibotSQLite")
                               ?? "Data Source=Dezibot.db";

        return services.AddSqlite<ApplicationDbContext>(connectionString,
            builder => builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    }
}