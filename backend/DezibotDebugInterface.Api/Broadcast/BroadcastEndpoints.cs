using System.Net;

using DezibotDebugInterface.Api.Broadcast.DezibotHubs;
using DezibotDebugInterface.Api.Broadcast.Models;
using DezibotDebugInterface.Api.Common.Constants;
using DezibotDebugInterface.Api.Common.DataAccess;
using DezibotDebugInterface.Api.Common.Models;

using Microsoft.AspNetCore.SignalR;

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

    private static async Task<IResult> HandleStateBroadcastDataAsync(StateBroadcastRequest request, IDezibotRepository dezibotRepository, IHubContext<DezibotHub, IDezibotHubClient> hubContext)
    {
        var dezibot = await dezibotRepository.UpdateAsync(request.Ip, request.Debuggables);
        await hubContext.Clients.All.SendDezibotUpdateAsync(dezibot);
        return Results.NoContent();
    }
    
    private static async Task<IResult> HandleLogBroadcastDataAsync(LogBroadcastRequest request, IDezibotRepository dezibotRepository, IHubContext<DezibotHub, IDezibotHubClient> hubContext)
    {
        var dezibot = await dezibotRepository.UpdateAsync(request.Ip, logs: [new Dezibot.LogEntry(request.TimestampUtc, request.LogLevel, request.Message)]);
        await hubContext.Clients.All.SendDezibotUpdateAsync(dezibot);
        return Results.NoContent();
    }
}