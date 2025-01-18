using System.Net;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.Constants;

using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.Endpoints.GetDezibot;

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
            .Produces<List<DezibotViewModel>>((int)HttpStatusCode.OK, ContentTypes.ApplicationJson)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ApplicationProblemJson)
            .WithOpenApi();

        endpoints.MapGet("api/dezibots/{ip}", GetDezibotByIpAsync)
            .WithName("Get Dezibot By Ip")
            .WithSummary("Returns a dezibot by its IP address.")
            .Produces<DezibotViewModel>((int)HttpStatusCode.OK, ContentTypes.ApplicationProblemJson)
            .ProducesProblem((int)HttpStatusCode.NotFound, ContentTypes.ApplicationProblemJson)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ApplicationProblemJson)
            .WithOpenApi();
        
        return endpoints;
    }

    private static async Task<IResult> GetAllDezibotsAsync(DezibotDbContext dbContext)
    {
        return Results.Ok(await dbContext.Dezibots.Select(dezibot => dezibot.ToDezibotViewModel()).ToListAsync());
    }

    private static async Task<IResult> GetDezibotByIpAsync(string ip, DezibotDbContext dbContext)
    {
        var dezibot = await dbContext.Dezibots.Where(dezibot => dezibot.Ip == ip).FirstOrDefaultAsync();
        return dezibot is null
            ? Results.NotFound()
            : Results.Ok(dezibot.ToDezibotViewModel());
    }
}