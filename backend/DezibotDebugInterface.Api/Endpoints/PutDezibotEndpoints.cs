using System.Net;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.Models;

namespace DezibotDebugInterface.Api.Endpoints;

/// <summary>
/// Endpoint for updating Dezibot data.
/// </summary>
public static class PutDezibotEndpoints
{
    /// <summary>
    /// Maps the PUT endpoints for Dezibots.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    public static void MapPutDezibotEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("api/dezibot/broadcast", UpdateDezibotAsync)
            .WithName("BroadcastDezibot")
            .WithSummary("Broadcast a message to all Dezibots.")
            .Produces<PutDezibotRequest>((int)HttpStatusCode.NoContent, "application/json")
            .ProducesProblem((int)HttpStatusCode.BadRequest, "application/problem+json")
            .ProducesProblem((int)HttpStatusCode.InternalServerError, "application/problem+json")
            .WithOpenApi();
    }

    private static async Task<IResult> UpdateDezibotAsync(PutDezibotRequest request, IDezibotRepository dezibotRepository)
    {
        var success = await dezibotRepository.SaveBroadcastDataAsync(request);
        return success ? Results.NoContent() : Results.Problem("Failed to save broadcast data.", statusCode: (int)HttpStatusCode.InternalServerError);
    }
}