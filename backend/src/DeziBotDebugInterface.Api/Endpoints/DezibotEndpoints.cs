using DeziBotDebugInterface.Api.Clients;
using DeziBotDebugInterface.Api.Endpoints.Common;
using DeziBotDebugInterface.Api.Endpoints.Requests;
using DeziBotDebugInterface.Api.Repositories;

namespace DeziBotDebugInterface.Api.Endpoints;

/// <summary>
/// Provides endpoints for 
/// </summary>
public class DezibotEndpoints : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/dezibot/", GetDezibot);
        endpoints.MapGet("/dezibot/{serialNumber}", GetDezibotByIdAsync);
        
        endpoints.MapPost("/dezibot/", AddDezibotAsync);
        endpoints.MapPost("/dezibot/broadcast", ReceiveBroadcastAsync);
        
        endpoints.MapPost("/dezibot/command", SendCommandAsync);
        endpoints.MapPost("/dezibot/command/{serialNumber}", SendCommandByIdAsync);
    }

    internal static IResult GetDezibot(IDezibotRepository dezibotRepository)
    {
        return Results.Json(dezibotRepository.GetDezibotsAsync());
    }
    
    internal static async Task<IResult> GetDezibotByIdAsync(IDezibotRepository dezibotRepository,string serialNumber)
    {
        var dezibot = await dezibotRepository.GetDezibotByIdAsync(serialNumber);
        return dezibot == null ? Results.NotFound() : Results.Json(dezibot);
    }
    
    internal static async Task<IResult> AddDezibotAsync(
        IDezibotRepository dezibotRepository,
        HttpProblemDetailsService problemDetailsService,
        AddDezibotRequest request)
    {
        var result = await dezibotRepository.AddDezibotAsync(request);
        return result.Match(
            serialNumber => Results.CreatedAtRoute($"/dezibot/{serialNumber}"),
            problemDetailsService.LogErrorsAndReturnProblem);
    }
    
    internal static async Task<IResult> ReceiveBroadcastAsync(
        IDezibotClient dezibotClient,
        HttpProblemDetailsService problemDetailsService,
        ReceiveBroadcastRequest request)
    {
        var result = await dezibotClient.ReceiveBroadcastAsync(request);
        return result.Match(
            _ => Results.Ok(),
            problemDetailsService.LogErrorsAndReturnProblem);
    }
    
    internal static async Task<IResult> SendCommandAsync(
        IDezibotClient dezibotClient,
        HttpProblemDetailsService problemDetailsService,
        SendCommandRequest request)
    {
        var result = await dezibotClient.SendCommandAsync(request);
        return result.Match(
            _ => Results.Ok(),
            problemDetailsService.LogErrorsAndReturnProblem);
    }
    
    internal static async Task<IResult> SendCommandByIdAsync(
        IDezibotClient dezibotClient,
        HttpProblemDetailsService problemDetailsService,
        SendCommandRequest request,
        string serialNumber)
    {
        var result = await dezibotClient.SendCommandByIdAsync(request, serialNumber);
        return result.Match(
            _ => Results.Ok(),
            problemDetailsService.LogErrorsAndReturnProblem);
    }
}