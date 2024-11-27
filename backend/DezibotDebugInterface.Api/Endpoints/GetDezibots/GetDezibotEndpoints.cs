using System.Net;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.Constants;

namespace DezibotDebugInterface.Api.Endpoints.GetDezibots;

/// <summary>
/// Defines the GET endpoints for dezibots.
/// </summary>
public static class GetDezibotEndpoints
{
    /// <summary>
    /// Maps the GET endpoints for dezibots.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    public static IEndpointRouteBuilder MapGetDezibotEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("api/dezibots", GetAllDezibotsAsync)
            .WithName("Get All Dezibots")
            .WithSummary("Returns all dezibots.")
            .Produces<IAsyncEnumerable<Dezibot>>((int)HttpStatusCode.OK, ContentTypes.JsonContentType)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ProblemContentType)
            .WithOpenApi();

        endpoints.MapGet("api/dezibots/{ip}", GetDezibotByIpAsync)
            .WithName("Get Dezibot By Ip")
            .WithSummary("Returns a dezibot by its IP address.")
            .Produces<Dezibot>((int)HttpStatusCode.OK, ContentTypes.ProblemContentType)
            .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypes.ProblemContentType)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ProblemContentType)
            .WithOpenApi();
        
        return endpoints;
    }

    private static IResult GetAllDezibotsAsync(DezibotDbContext dbContext)
    {
        return Results.Ok(dbContext.Dezibots.ToAsyncEnumerable());
    }

    private static async Task<IResult> GetDezibotByIpAsync(string ip, DezibotDbContext dbContext)
    {
        var dezibot = await dbContext.Dezibots.FindAsync(ip);
        return dezibot is null
            ? Results.NotFound()
            : Results.Ok(dezibot);
    }
}