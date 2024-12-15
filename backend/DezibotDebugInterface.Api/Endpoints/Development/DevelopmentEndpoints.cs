using DezibotDebugInterface.Api.DataAccess;

using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.Endpoints.Development;

/// <summary>
/// Provides development endpoints.
/// </summary>
public static class DevelopmentEndpoints
{
    /// <summary>
    /// Maps the development endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapDevelopmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/resetDatabase", async (DezibotDbContext dbContext) =>
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();

            return Results.Ok("Database reset.");
        }).WithSummary("Reset Database").WithOpenApi();
    
        endpoints.MapDelete("/api/dezibot/{ip}", async (DezibotDbContext dbContext, string ip) =>
        {
            var dezibot = await dbContext.Dezibots.Where(dezibot => dezibot.Ip == ip).FirstOrDefaultAsync();
            if (dezibot is null)
            {
                return Results.NotFound();
            }

            dbContext.Dezibots.Remove(dezibot);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).WithSummary("Delete Dezibot by IP").WithOpenApi();

        endpoints.MapPut("/api/dezibot", async (DezibotDbContext dbContext) =>
        {
            var dezibot = DezibotFactory.CreateDezibots(
                amount: 1,
                classes: DezibotFactory.CreateClasses(
                    amount: 10,
                    properties: DezibotFactory.CreateProperties(
                        amount: 10,
                        timeValues: DezibotFactory.CreateTimeValues(amount: 10))),
                logs: DezibotFactory.CreateLogEntries(amount: 1000))[0];
        
            await dbContext.Dezibots.AddRangeAsync(dezibot);
            await dbContext.SaveChangesAsync();
        }).WithSummary("Create Stress Test Dezibot").WithOpenApi();
    }
}