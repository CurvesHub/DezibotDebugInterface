using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.GetDezibots;
using DezibotDebugInterface.Api.Endpoints.UpdateDezibot;
using DezibotDebugInterface.Api.SignalRHubs;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

namespace DezibotDebugInterface.Api;

/// <summary>
/// Provides extension methods for mapping project endpoints.
/// </summary>
public static class EndpointMap
{
    /// <summary>
    /// Maps the project endpoints.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapProjectEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapOpenApiRelatedEndpoints()
            .MapGetDezibotEndpoints()
            .MapUpdateDezibotEndpoint()
            .MapHub<DezibotHub>("/dezibot-hub");
    }

    public static void MapDevelopmentEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/resetDatabase", async (DezibotDbContext dbContext) =>
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();

            return Results.Ok("Database reset.");
        }).WithOpenApi();
    
        endpoints.MapDelete("/api/dezibot/{ip}", async (DezibotDbContext dbContext, string ip) =>
        {
            var dezibot = await dbContext.Dezibots.FindAsync(ip);
            if (dezibot is null)
            {
                return Results.NotFound();
            }

            dbContext.Dezibots.Remove(dezibot);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).WithOpenApi();
    }
    
    /// <summary>
    /// Maps the error endpoint.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    public static void MapErrorEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.Map("/error", HandleException);
    }
    
    private static IEndpointRouteBuilder MapOpenApiRelatedEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapOpenApi();
        endpoints.MapScalarApiReference(options => 
            options.Servers = [new ScalarServer("http://localhost:5160", "Development")]);
        
        endpoints.Map("/", () => Results.Redirect("/scalar/v1"));
        endpoints.Map("/api", () => Results.Redirect("/scalar/v1"));
        
        return endpoints;
    }

    private static IResult HandleException(HttpContext context)
    {
        var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (ex is null)
        {
            return Results.Problem(detail: "An error occurred.", statusCode: 500);
        }

        if (ex.InnerException is null)
        {
            return Results.Problem(detail: ex.Message, statusCode: 500);
        }
    
        var message = ex.Message;
    
        var innerEx = ex.InnerException;
        while (innerEx != null)
        {
            ex = innerEx;
            innerEx = ex.InnerException;
        }
    
        return Results.Problem(detail: message + $" Detailed Error: {ex.Message}", statusCode: 500);
    }
}