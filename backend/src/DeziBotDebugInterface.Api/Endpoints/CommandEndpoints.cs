using DeziBotDebugInterface.Api.Endpoints.Common;
using DeziBotDebugInterface.Api.Repositories;

namespace DeziBotDebugInterface.Api.Endpoints;

/// <summary>
/// Provides endpoints for sending commands to Dezibots.
/// </summary>
public class CommandEndpoints : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/command/", GetCommandAsync);
        endpoints.MapGet("/command/{serialNumber}", GetCommandByIdAsync);
    }

    internal static IResult GetCommandAsync(ICommandRepository commandRepository)
    {
        return Results.Json(commandRepository.GetCommandsAsync());
    }
    
    internal static IResult GetCommandByIdAsync(ICommandRepository commandRepository, string serialNumber)
    {
        return Results.Json(commandRepository.GetCommandsByIdAsync(serialNumber));
    }
}