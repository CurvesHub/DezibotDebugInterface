using DezibotDebugInterface.Api.Endpoints.Sessions;
using DezibotDebugInterface.Api.Endpoints.SignalR;
using DezibotDebugInterface.Api.Endpoints.UpdateDezibot;

using Microsoft.AspNetCore.Diagnostics;

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
            .MapGetSessionEndpoints()
            .MapCreateSessionEndpoints()
            .MapDeleteSessionEndpoints()
            .MapUpdateDezibotEndpoint()
            .MapHub<DezibotHub>("/api/dezibot-hub").AllowAnonymous();
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