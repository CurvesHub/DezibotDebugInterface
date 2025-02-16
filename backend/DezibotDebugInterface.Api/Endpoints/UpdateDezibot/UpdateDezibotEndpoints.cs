using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Endpoints.Common;
using DezibotDebugInterface.Api.Endpoints.SignalR;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using OneOf;

using Serilog;

namespace DezibotDebugInterface.Api.Endpoints.UpdateDezibot;

/// <summary>
/// Defines an endpoint for dezibots to send state and log data to.
/// </summary>
public static class UpdateDezibotEndpoints
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() {PropertyNameCaseInsensitive = true};

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
            .WithOpenApi();
        
        return endpoints;
    }
    
    private static async Task<IResult> HandleDezibotUpdateAsync(
        HttpContext httpContext,
        ApplicationDbContext dbContext,
        IHubContext<DezibotHub, IDezibotHubClient> hubContext)
    {
        // Validate the request body
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
                detail: "The IP address must not be null or empty.",
                statusCode: (int)HttpStatusCode.BadRequest);
        }

        // Handle the session association
        var activeUsedSessions = await dbContext.Sessions
            .Include(session => session.Dezibots.Where(dezibot => dezibot.Ip == ip))
            .ThenInclude(dezibot => dezibot.Classes)
            .ThenInclude(@class => @class.Properties)
            .ThenInclude(property => property.Values)
            .Include(session => session.SessionClientConnections)
            .ThenInclude(sessionClientConnection => sessionClientConnection.Client)
            .Where(session => session.SessionClientConnections.Any(sessionClientConnection => sessionClientConnection.ReceiveUpdates))
            .ToListAsync();

        if (activeUsedSessions.Count is 0)
        {
            // No active sessions -> ignore the data
            // Since the bot can easily be restarted we won't have to care about data loss
            return Results.NoContent();
        }
        
        // Update Dezibots tied to active sessions
        foreach (var session in activeUsedSessions)
        {
            var dezibot = session.Dezibots.FirstOrDefault(bot => bot.Ip == ip);
            
            if (dezibot is null)
            {
                // Create a new Dezibot for this specific session
                dezibot = new Dezibot { Ip = ip, SessionId = session.Id };
                await dbContext.AddAsync(dezibot);
            }

            dezibot.UpdateDezibot(request.Value);
            
            // Notify the clients about the updated Dezibot
            var signalRClientConnections = session.SessionClientConnections
                .Where(connection => connection.ReceiveUpdates)
                .Select(connection => connection.Client!.ConnectionId)
                .ToList();
            
            await hubContext.Clients
                .Clients(signalRClientConnections)
                .DezibotUpdated(dezibot.ToDezibotViewModel());
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
            updateLogsRequest = JsonSerializer.Deserialize<UpdateDezibotLogsRequest>(body, options: JsonSerializerOptions);
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
            updateStatesRequest = JsonSerializer.Deserialize<UpdateDezibotStatesRequest>(body, options: JsonSerializerOptions);
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
        dezibot.Logs.Add(new LogEntry
        {
            TimestampUtc = dezibot.LastConnectionUtc,
            LogLevel = request.LogLevel,
            ClassName = request.ClassName,
            Message = request.Message,
            Data = request.Data,
        });
    }

    private static void UpdateClassStates(this Dezibot dezibot, UpdateDezibotStatesRequest request)
    {
        var updatedClasses = request.Data.Select(state => new Class
        {
            Name = state.Key,
            Properties = state.Value.Select(property => new Property
            {
                Name = property.Key,
                Values = [new TimeValue { TimestampUtc = dezibot.LastConnectionUtc, Value = property.Value }]
            }).ToList()
        }).ToList();

        if (dezibot.Classes.Count is 0)
        {
            dezibot.Classes.AddRange(updatedClasses);
            return;
        }

        foreach (var updatedClass in updatedClasses)
        {
            var existingClass = dezibot.Classes.FirstOrDefault(@class => @class.Name == updatedClass.Name);

            if (existingClass is null)
            {
                dezibot.Classes.Add(updatedClass);
                continue;
            }

            foreach (var newProperty in updatedClass.Properties)
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