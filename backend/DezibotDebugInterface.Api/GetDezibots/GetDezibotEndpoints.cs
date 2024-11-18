using System.Net;

using DezibotDebugInterface.Api.Common.Constants;
using DezibotDebugInterface.Api.Common.DataAccess;
using DezibotDebugInterface.Api.Common.Models;

namespace DezibotDebugInterface.Api.GetDezibots;

/// <summary>
///     Endpoint for retrieving Dezibot data via GET requests.
/// </summary>
public static class GetDezibotEndpoints
{
    /// <summary>
    ///     Maps the GET endpoints for Dezibots.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    public static void MapGetDezibotEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("api/dezibots", GetAllDezibotsAsync)
            .WithName("GetAllDezibots")
            .WithSummary("Get all Dezibots.")
            .Produces<IAsyncEnumerable<Dezibot>>((int)HttpStatusCode.OK, ContentTypes.JsonContentType)
            .Produces<Dezibot>((int)HttpStatusCode.OK, ContentTypes.JsonContentType)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ProblemContentType)
            .WithOpenApi();

        endpoints.MapGet("api/dezibots/{ip}", GetDezibotByIpAsync)
            .WithName("GetDezibotByIp")
            .WithSummary("Get a Dezibot by IP.")
            .Produces<Dezibot>((int)HttpStatusCode.OK, ContentTypes.ProblemContentType)
            .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypes.ProblemContentType)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ProblemContentType)
            .WithOpenApi();
    }

    private static IResult GetAllDezibotsAsync(IDezibotRepository dezibotRepository)
    {
        return Results.Ok(dezibotRepository.GetAllDezibotsAsync());
    }

    private static async Task<IResult> GetDezibotByIpAsync(string ip, IDezibotRepository dezibotRepository)
    {
        Dezibot? dezibot = await dezibotRepository.GetByIpAsync(ip);
        return dezibot is null
            ? Results.NotFound()
            : Results.Ok(dezibot);
    }
}