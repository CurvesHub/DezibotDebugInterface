using System.Net;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Endpoints.Common;

namespace DezibotDebugInterface.Api.Endpoints.Sessions;

/// <summary>
/// Defines the POST endpoints for sessions.
/// </summary>
public static class CreateSessionEndpoints
{
    /// <summary>
    /// Maps the POST session endpoints to the provided endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    /// <returns>The endpoint route builder with the session endpoints mapped to it.</returns>
    public static IEndpointRouteBuilder MapCreateSessionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/session", CreateSessionAsync)
            .WithName("Create Session")
            .WithSummary("Creates a new session.")
            .Accepts<CreateSessionRequest>(ContentTypes.ApplicationJson)
            .Produces<SessionIdentifier>((int)HttpStatusCode.Created, ContentTypes.ApplicationJson)
            .WithOpenApi();

        return endpoints;
    }
    
    private static async Task<IResult> CreateSessionAsync(ApplicationDbContext dbContext, CreateSessionRequest request)
    {
        var session = new Session(string.IsNullOrWhiteSpace(request.Name) ? null : request.Name);

        await dbContext.Sessions.AddAsync(session);
        await dbContext.SaveChangesAsync();

        return Results.Created($"/api/session/{session.Id}", session.ToSessionIdentifier());
    }
}