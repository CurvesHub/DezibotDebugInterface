using DeziBotDebugInterface.Api.Endpoints.Common;
using DeziBotDebugInterface.Api.Repositories;

namespace DeziBotDebugInterface.Api.Endpoints;

public class SensorEndpoints : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/sensor/", GetSensorAsync);
        endpoints.MapGet("/sensor/{serialNumber}", GetSensorByIdAsync);
    }
    
    internal static IResult GetSensorAsync(ISensorRepository sensorRepository)
    {
        return Results.Json(sensorRepository.GetSensorsAsync());
    }
    
    internal static IResult GetSensorByIdAsync(ISensorRepository sensorRepository, string serialNumber)
    {
        return Results.Json(sensorRepository.GetSensorByIdAsync(serialNumber));
    }
}