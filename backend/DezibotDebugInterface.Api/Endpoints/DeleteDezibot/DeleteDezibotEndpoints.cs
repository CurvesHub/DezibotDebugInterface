using System.Net;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.Constants;

using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.Endpoints.DeleteDezibot;

/// <summary>
/// Defines the DELETE endpoints for dezibots.
/// </summary>
public static class DeleteDezibotEndpoints
{
    /// <summary>
    /// Maps the DELETE endpoints for dezibots.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static IEndpointRouteBuilder MapDeleteDezibotEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/dezibots", DeleteAllDezibotsAsync)
            .WithName("Delete All Dezibots")
            .WithSummary("Deletes all dezibots.")
            .Produces<string>((int)HttpStatusCode.OK, ContentTypes.ApplicationJson)
            .WithOpenApi();
        
        endpoints.MapDelete("/api/dezibot/{ip}", DeleteDezibotAsync)
            .WithName("Delete Dezibot By Ip")
            .WithSummary("Deletes a dezibot by its IP address.")
            .Produces<string>((int)HttpStatusCode.OK, ContentTypes.ApplicationJson)
            .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypes.ApplicationProblemJson)
            .WithOpenApi();
        
        return endpoints;
    }
    
    private static async Task<IResult> DeleteAllDezibotsAsync(DezibotDbContext dbContext)
    {
        var deletedRows = await dbContext.Dezibots.ExecuteDeleteAsync();
        return Results.Ok($"Deleted {deletedRows} rows.");
    }
    
    private static async Task<IResult> DeleteDezibotAsync(string ip, DezibotDbContext dbContext)
    {
        var dezibot = await dbContext.Dezibots.Where(dezibot => dezibot.Ip == ip).FirstOrDefaultAsync();
        if (dezibot is null)
        {
            return Results.NotFound();
        }

        dbContext.Dezibots.Remove(dezibot);
        await dbContext.SaveChangesAsync();

        return Results.Ok($"Deleted dezibot with IP {ip}.");
    }
}