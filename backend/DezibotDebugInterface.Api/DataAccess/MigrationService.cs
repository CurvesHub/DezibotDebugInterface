using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.DataAccess;

/// <summary>
/// Provides a service for ensuring the database is migrated. 
/// </summary>
public static class MigrationService
{
    /// <summary>
    /// Ensures the database is migrated.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public static async Task MigrateDatabaseAsync(IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}