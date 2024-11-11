using System.Globalization;
using System.Net;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Endpoints.Models;

using Component = DezibotDebugInterface.Api.Endpoints.Models.Component;
using DebugValue = DezibotDebugInterface.Api.Endpoints.Models.DebugValue;
using Log = DezibotDebugInterface.Api.Endpoints.Models.Log;
using Property = DezibotDebugInterface.Api.Endpoints.Models.Property;

namespace DezibotDebugInterface.Api.Endpoints;

/// <summary>
/// Endpoint for retrieving Dezibot data.
/// </summary>
public static class GetDezibotEndpoints
{
    /// <summary>
    /// Maps the GET endpoints for Dezibots.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    public static void MapGetDezibotEndpoints(this IEndpointRouteBuilder endpoints)
    {
        const string problemJson = "application/problem+json";
        endpoints.MapGet("api/dezibot", GetAllDezibotsAsync)
            .WithName("GetAllDezibots")
            .WithSummary("Get all Dezibots.")
            .Produces<GetDezibotResponse>((int)HttpStatusCode.OK, "application/json")
            .ProducesProblem((int)HttpStatusCode.InternalServerError, problemJson)
            .WithOpenApi();
        
        endpoints.MapGet("api/dezibot/{ip}", GetDezibotByIpAsync)
            .WithName("GetDezibotByIp")
            .WithSummary("Get a Dezibot by IP.")
            .Produces<GetDezibotResponse>((int)HttpStatusCode.OK, problemJson)
            .ProducesProblem((int)HttpStatusCode.NotFound, problemJson)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, problemJson)
            .WithOpenApi();
    }
    
    private static IResult GetAllDezibotsAsync(IDezibotRepository dezibotRepository)
    {
        IAsyncEnumerable<Dezibot> dezibots = dezibotRepository.GetAllDezibotsAsync();
        return Results.Ok(dezibots.Select(ToResponse));
    }

    private static async Task<IResult> GetDezibotByIpAsync(string ip, IDezibotRepository dezibotRepository)
    {
        var dezibot = await dezibotRepository.GetDezibotByIpAsync(ip);
        return dezibot is null
            ? Results.NotFound()
            : Results.Ok(ToResponse(dezibot));
    }
    
    private static GetDezibotResponse ToResponse(Dezibot dezibot)
    {
        return new GetDezibotResponse(
            Ip: dezibot.Ip,
            LastConnectionUtc: dezibot.LastConnectionUtc.ToString("O", CultureInfo.InvariantCulture),
            Components: dezibot.Components.Select(component => new Component(
                Name: component.Name,
                Properties: component.Properties.Select(property => new Property(
                    Name: property.Name,
                    Values: property.Values.Select(debugValue => new DebugValue(
                        TimestampUtc: debugValue.TimestampUtc.ToString("O", CultureInfo.InvariantCulture),
                        Value: debugValue.Value)))),
                Logs: component.Logs.Select(log => new Log(
                    Message: log.Message,
                    Values: log.Values.Select(debugValue => new DebugValue(
                        TimestampUtc: debugValue.TimestampUtc.ToString("O", CultureInfo.InvariantCulture),
                        Value: debugValue.Value)))))));
    }
}