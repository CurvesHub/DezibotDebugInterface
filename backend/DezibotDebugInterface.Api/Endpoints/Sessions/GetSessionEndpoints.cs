using System.Net;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.Common;

using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.Endpoints.Sessions;

/// <summary>
/// Defines the GET endpoints for sessions.
/// </summary>
public static class GetSessionEndpoints
{
    /// <summary>
    /// Maps the GET session endpoints to the provided endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    /// <returns>The endpoint route builder with the session endpoints mapped to it.</returns>
    public static IEndpointRouteBuilder MapGetSessionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/sessions/available", GetAllSessionIdentifiersAsync)
            .WithName("Get All Session Identifiers")
            .WithSummary("Gets all session identifiers.")
            .Produces<List<SessionIdentifier>>((int)HttpStatusCode.OK, ContentTypes.ApplicationJson)
            .WithOpenApi();
        
        endpoints.MapGet("/api/sessions", GetAllSessionsAsync)
            .WithName("Get All Sessions")
            .WithSummary("Gets all sessions.")
            .Produces<List<SessionViewModel>>((int)HttpStatusCode.OK, ContentTypes.ApplicationJson)
            .WithOpenApi();
        
        endpoints.MapGet("/api/session/{id:int}", GetSessionByIdAsync)
            .WithName("Get Session By Id")
            .WithSummary("Gets a session by its ID.")
            .Produces<SessionViewModel>((int)HttpStatusCode.OK, ContentTypes.ApplicationJson)
            .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypes.ApplicationProblemJson)
            .WithOpenApi();
        
        endpoints.MapGet("api/session/{id:int}/dezibot/{ip}", GetDezibotFromSessionAsync)
            .WithName("Get Dezibot By Ip")
            .WithSummary("Returns a dezibot by its IP address.")
            .Produces<DezibotViewModel>((int)HttpStatusCode.OK, ContentTypes.ApplicationProblemJson)
            .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypes.ApplicationProblemJson)
            .WithOpenApi();
        
        return endpoints;
    }
    
    private static async Task<IResult> GetAllSessionIdentifiersAsync(ApplicationDbContext dbContext)
    {
        var sessionIdentifiers = await dbContext.Sessions
            .Select(session => session.ToSessionIdentifier())
            .ToListAsync();

        return Results.Ok(sessionIdentifiers);
    }
    
    private static async Task<IResult> GetAllSessionsAsync(ApplicationDbContext dbContext)
    {
        var sessions = await dbContext.Sessions
            .Include(session => session.Dezibots)
            .ThenInclude(dezibot => dezibot.Classes)
            .ThenInclude(@class => @class.Properties)
            .ThenInclude(property => property.Values)
            .Select(session => session.ToSessionViewModel())
            .ToListAsync();

        return Results.Ok(sessions);
    }
    
    private static async Task<IResult> GetSessionByIdAsync(ApplicationDbContext dbContext, int id)
    {
        var session = await dbContext.Sessions
            .Include(session => session.Dezibots)
            .ThenInclude(dezibot => dezibot.Classes)
            .ThenInclude(@class => @class.Properties)
            .ThenInclude(property => property.Values)
            .Where(session => session.Id == id)
            .Select(session => session.ToSessionViewModel())
            .FirstOrDefaultAsync();

        if (session is null)
        {
            return Results.Problem(
                detail: $"Session with ID {id} not found.",
                statusCode: (int)HttpStatusCode.NotFound);
        }
        
        return Results.Ok(session);
    }
    
    private static async Task<IResult> GetDezibotFromSessionAsync(ApplicationDbContext dbContext, int id, string ip)
    {
        var session = await dbContext.Sessions
            .Include(session => session.Dezibots.Where(dezibot => dezibot.Ip == ip))
            .ThenInclude(dezibot => dezibot.Classes)
            .ThenInclude(@class => @class.Properties)
            .ThenInclude(property => property.Values)
            .Where(session => session.Id == id)
            .Select(session => session.ToSessionViewModel())
            .FirstOrDefaultAsync();

        if (session is null)
        {
            return Results.Problem(
                detail: $"Session with ID {id} not found.",
                statusCode: (int)HttpStatusCode.NotFound);
        }
        
        var dezibot = session.Dezibots.FirstOrDefault(dezibot => dezibot.Ip == ip);
        if (dezibot is null)
        {
            return Results.Problem(
                detail: $"The dezibot with IP {ip} was not found in session with ID {id}.",
                statusCode: (int)HttpStatusCode.NotFound);
        }
        
        return Results.Ok(dezibot);
    }
}