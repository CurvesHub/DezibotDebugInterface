using System.Net;

using DezibotDebugInterface.Api.Broadcast.Models;
using DezibotDebugInterface.Api.Common.Constants;

namespace DezibotDebugInterface.Api.Broadcast;

/// <summary>
/// Endpoint for listening to broadcast requests from Dezibots.
/// </summary>
public static class BroadcastEndpoints
{
    /// <summary>
    /// Maps the broadcast endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    public static void MapBroadcastEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("api/broadcast/states", HandleStateBroadcastDataAsync)
            .WithName("BroadcastStates")
            .WithSummary("Listens for a broadcast request with state data and handles it.")
            .Accepts<StateBroadcastRequest>(ContentTypes.JsonContentType)
            .Produces((int)HttpStatusCode.NoContent)
            .ProducesProblem((int)HttpStatusCode.BadRequest, ContentTypes.ProblemContentType)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ProblemContentType)
            .WithOpenApi();
        
        endpoints.MapPut("api/broadcast/logs", HandleLogBroadcastDataAsync)
            .WithName("BroadcastLogs")
            .WithSummary("Listens for a broadcast request with log data and handles it.")
            .Accepts<LogBroadcastRequest>(ContentTypes.JsonContentType)
            .Produces((int)HttpStatusCode.NoContent)
            .ProducesProblem((int)HttpStatusCode.BadRequest, ContentTypes.ProblemContentType)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ProblemContentType)
            .WithOpenApi();
    }

    private static async Task<IResult> HandleStateBroadcastDataAsync(StateBroadcastRequest request, IBroadcastService broadcastService)
    {
        await broadcastService.HandleStateBroadcastDataAsync(request);
        return Results.NoContent();
    }
    
    private static async Task<IResult> HandleLogBroadcastDataAsync(LogBroadcastRequest request, IBroadcastService broadcastService)
    {
        await broadcastService.HandleLogBroadcastDataAsync(request);
        return Results.NoContent();
    }
}