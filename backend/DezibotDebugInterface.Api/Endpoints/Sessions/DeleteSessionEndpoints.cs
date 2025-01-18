using System.Net;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.Common;

using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.Endpoints.Sessions;

/// <summary>
/// Defines the DELETE endpoints for sessions.
/// </summary>
public static class DeleteSessionEndpoints
{
    /// <summary>
    /// Maps the DELETE session endpoints to the provided endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    /// <returns>The endpoint route builder with the session endpoints mapped to it.</returns>
    public static IEndpointRouteBuilder MapDeleteSessionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/sessions", DeleteAllSessionsAsync)
            .WithName("Delete All Sessions")
            .WithSummary("Deletes all sessions.")
            .Produces<string>((int)HttpStatusCode.OK, ContentTypes.ApplicationJson)
            .WithOpenApi();
        
        endpoints.MapDelete("/api/session/{id:int}", DeleteSessionByIdAsync)
            .WithName("Delete Session By Id")
            .WithSummary("Deletes a session by its ID.")
            .Produces<string>((int)HttpStatusCode.OK, ContentTypes.ApplicationJson)
            .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypes.ApplicationProblemJson)
            .ProducesProblem((int)HttpStatusCode.Conflict, ContentTypes.ApplicationProblemJson)
            .WithOpenApi();
        
        endpoints.MapDelete("/api/session/{id:int}/dezibot/{ip}", DeleteDezibotAsync)
            .WithName("Delete Dezibot By Ip From Session by Id")
            .WithSummary("Deletes a dezibot by its IP address from a session by its ID.")
            .Produces<string>((int)HttpStatusCode.OK, ContentTypes.ApplicationJson)
            .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypes.ApplicationProblemJson)
            .WithOpenApi();
        
        return endpoints;
    }
    
    private static async Task<IResult> DeleteAllSessionsAsync(DezibotDbContext dbContext)
    {
        await dbContext.Sessions.ExecuteDeleteAsync();
        return Results.Ok("Deleted all sessions.");
    }
    
    private static async Task<IResult> DeleteSessionByIdAsync(DezibotDbContext dbContext, int id)
    {
        var session = await dbContext.Sessions.FirstOrDefaultAsync(session => session.Id == id);
        if (session is null)
        {
            return Results.Problem(
                detail: $"Session with ID {id} not found.",
                statusCode: (int)HttpStatusCode.NotFound);
        }

        if (session.IsActive)
        {
            return Results.Problem(
                detail: "Cannot delete an active session.",
                statusCode: (int)HttpStatusCode.Conflict);
        }

        dbContext.Sessions.Remove(session);
        await dbContext.SaveChangesAsync();

        return Results.Ok($"Deleted session with ID {id}.");
    }
    
    private static async Task<IResult> DeleteDezibotAsync(DezibotDbContext dbContext, int id, string ip)
    {
        var session = await dbContext.Sessions
            .Include(session => session.Dezibots.Where(dezibot => dezibot.Ip == ip))
            .FirstOrDefaultAsync(session => session.Id == id);

        if (session is null)
        {
            return Results.Problem(
                detail: $"The session with ID {id} does not exist.",
                statusCode: (int)HttpStatusCode.NotFound);
        }
        
        var dezibot = session.Dezibots.FirstOrDefault(dezibot => dezibot.Ip == ip);
        if (dezibot is null)
        {
            return Results.Problem(
                detail: $"The dezibot with IP {ip} does not exist in session with ID {id}.",
                statusCode: (int)HttpStatusCode.NotFound);
        }
        
        dbContext.Dezibots.Remove(dezibot);
        await dbContext.SaveChangesAsync();
        
        return Results.Ok($"Deleted dezibot with IP {ip} from session with ID {id}.");
    }
}