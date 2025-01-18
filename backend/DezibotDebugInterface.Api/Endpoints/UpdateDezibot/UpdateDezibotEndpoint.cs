using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Endpoints.Constants;
using DezibotDebugInterface.Api.Endpoints.GetDezibot;
using DezibotDebugInterface.Api.Sessions;
using DezibotDebugInterface.Api.SignalRHubs;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using OneOf;

using Serilog;

namespace DezibotDebugInterface.Api.Endpoints.UpdateDezibot;

/// <summary>
/// Defines an endpoint for dezibots to send state and log data to.
/// </summary>
public static class UpdateDezibotEndpoint
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() {PropertyNameCaseInsensitive = true};

    /// <summary>
    /// Maps the update dezibot endpoint to the provided endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    public static IEndpointRouteBuilder MapUpdateDezibotEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("api/dezibot/update", HandleDezibotUpdateAsync)
            .WithName("Update Dezibot")
            .WithSummary("Accepts state and log data from a dezibot and updates the database.")
            .Accepts<UpdateDezibotLogsRequest>(ContentTypes.ApplicationJson)
            .Accepts<UpdateDezibotStatesRequest>(ContentTypes.ApplicationJson)
            .Produces((int)HttpStatusCode.NoContent)
            .ProducesProblem((int)HttpStatusCode.BadRequest, ContentTypes.ApplicationProblemJson)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ApplicationProblemJson)
            .WithOpenApi();
        
        return endpoints;
    }
    
    private static async Task<IResult> HandleDezibotUpdateAsync(
        HttpContext httpContext,
        DezibotDbContext dbContext,
        IHubContext<DezibotHub, IDezibotHubClient> hubContext,
        ISessionStore sessionStore)
    {
        // Validate the request body.
        var body = await new StreamReader(httpContext.Request.Body).ReadToEndAsync();
        var request = TryDeserializeRequests(body);

        if (request is null)
        {
            return Results.Problem(
                detail: "The request must contain either state or log data.",
                statusCode: (int)HttpStatusCode.BadRequest);
        }
        
        var ip = request.Value.Match(
            updateLogsRequest => updateLogsRequest.Ip,
            updateStatesRequest => updateStatesRequest.Ip);

        if (string.IsNullOrWhiteSpace(ip))
        {
            return Results.Problem(
                detail: "The request must contain a valid ip address.",
                statusCode: (int)HttpStatusCode.BadRequest);
        }

        var activeSessions = await sessionStore.GetActiveSessionAsync();

        if (activeSessions.Count is 0)
        {
            // No active sessions, look for or create a session-less Dezibot
            var dezibotWithoutSession = await dbContext.Dezibots
                .FirstOrDefaultAsync(dezibot => dezibot.Ip == ip && dezibot.SessionId == null);

            if (dezibotWithoutSession is null)
            {
                // Create a new session-less Dezibot
                dezibotWithoutSession = new Dezibot(ip);
                await dbContext.AddAsync(dezibotWithoutSession);
            }

            dezibotWithoutSession.UpdateDezibot(request.Value);
            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        }
        
        // Update Dezibots tied to active sessions
        foreach (var activeSession in activeSessions)
        {
            var dezibot = await dbContext.Dezibots
                .FirstOrDefaultAsync(bot => bot.Ip == ip && bot.SessionId == activeSession.Id);
            
            if (dezibot is null)
            {
                // Create a new Dezibot for this specific session
                dezibot = new Dezibot(ip, activeSession.Id);
                await dbContext.AddAsync(dezibot);
            }

            dezibot.UpdateDezibot(request.Value);
            await hubContext.Clients
                .Client(activeSession.ClientConnectionId)
                .SendDezibotUpdateAsync(dezibot.ToDezibotViewModel());
        }

        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
    
    [SuppressMessage("ReSharper", "ConstantConditionalAccessQualifier", Justification = "The request is checks for null are necessary.")]
    private static OneOf<UpdateDezibotLogsRequest, UpdateDezibotStatesRequest>? TryDeserializeRequests(string body)
    {
        UpdateDezibotLogsRequest? updateLogsRequest = null;
        try
        {
            updateLogsRequest = JsonSerializer.Deserialize<UpdateDezibotLogsRequest>(body, options: _jsonSerializerOptions);
        }
        catch (JsonException ex)
        {
            Log.Information(ex, "Failed to deserialize the request body for logs.");
        }
        
        if (updateLogsRequest?.Ip is not null && updateLogsRequest?.ClassName is not null)
        {
            return updateLogsRequest;
        }
        
        UpdateDezibotStatesRequest? updateStatesRequest = null;
        try
        {
            updateStatesRequest = JsonSerializer.Deserialize<UpdateDezibotStatesRequest>(body, options: _jsonSerializerOptions);
        }
        catch (JsonException ex)
        {
            Log.Information(ex, "Failed to deserialize the request body for states.");
        }

        if (updateStatesRequest?.Ip is not null && updateStatesRequest?.Data is not null)
        {
            return updateStatesRequest;
        }

        return null;
    }
    
    private static void UpdateDezibot(this Dezibot dezibot, OneOf<UpdateDezibotLogsRequest, UpdateDezibotStatesRequest> request)
    {
        dezibot.LastConnectionUtc = DateTimeOffset.UtcNow;
        request.Switch(dezibot.AddLogs, dezibot.UpdateClassStates);
    }
    
    private static void AddLogs(this Dezibot dezibot, UpdateDezibotLogsRequest request)
    {
        dezibot.Logs.Add(new LogEntry(
            dezibot.LastConnectionUtc,
            request.LogLevel,
            request.ClassName,
            request.Message,
            request.Data));
    }

    private static void UpdateClassStates(this Dezibot dezibot, UpdateDezibotStatesRequest request)
    {
        var newClasses = request.Data.Select(state => new Class(
            name: state.Key,
            properties: state.Value.Select(property => new Property(
                name: property.Key,
                values: [new TimeValue(dezibot.LastConnectionUtc, property.Value)])).ToList())).ToList();

        if (dezibot.Classes.Count is 0)
        {
            dezibot.Classes.AddRange(newClasses);
            return;
        }

        foreach (var newClass in newClasses)
        {
            var existingClass = dezibot.Classes.FirstOrDefault(@class => @class.Name == newClass.Name);

            if (existingClass is null)
            {
                dezibot.Classes.Add(newClass);
                continue;
            }

            foreach (var newProperty in newClass.Properties)
            {
                var existingProperty = existingClass.Properties.FirstOrDefault(property => property.Name == newProperty.Name);

                if (existingProperty is null)
                {
                    existingClass.Properties.Add(newProperty);
                    continue;
                }

                existingProperty.Values.AddRange(newProperty.Values);
            }
        }
    }
}