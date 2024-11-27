using System.Net;
using System.Text.Json;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.Constants;
using DezibotDebugInterface.Api.SignalRHubs;

using Microsoft.AspNetCore.SignalR;

using OneOf;

using Serilog;

namespace DezibotDebugInterface.Api.Endpoints.UpdateDezibot;

/// <summary>
/// Defines an endpoint for dezibots to send state and log data to.
/// </summary>
public static class UpdateDezibotEndpoint
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions(){PropertyNameCaseInsensitive = true};

    /// <summary>
    /// Maps the update dezibot endpoint to the provided endpoint route builder.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoints to.</param>
    public static void MapUpdateDezibotEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("api/dezibot/update", UpdateDezibotAsync)
            .WithName("Update Dezibot")
            .WithSummary("Accepts state and log data from a dezibot and updates the database.")
            .Accepts<UpdateDezibotLogsRequest>(ContentTypes.JsonContentType)
            .Accepts<UpdateDezibotStatesRequest>(ContentTypes.JsonContentType)
            .Produces((int)HttpStatusCode.NoContent)
            .ProducesProblem((int)HttpStatusCode.BadRequest, ContentTypes.ProblemContentType)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ProblemContentType)
            .WithOpenApi();
    }
    
    private static async Task<IResult> UpdateDezibotAsync(
        HttpContext httpContext,
        DezibotDbContext dbContext,
        IHubContext<DezibotHub, IDezibotHubClient> hubContext)
    {
        // TODO: Session Handling - Check if the request is part of an existing session. Otherwise store it without a session or dump it?
        // Implement a session model and managent and migrate the database
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

        var dezibot = await dbContext.Dezibots.FindAsync(ip);
        if (dezibot is null)
        {
            dezibot = new Dezibot(ip, DateTime.UtcNow);
            dbContext.Add(dezibot);
        }
        
        dezibot.LastConnectionUtc = DateTime.UtcNow;
        /*dezibot.LastConnectionUtc = request.Value.Match(
            updateLogsRequest => updateLogsRequest.TimestampUtc,
            updateStatesRequest => updateStatesRequest.TimestampUtc);*/

        request.Value.Switch(
            updateLogsRequest => dezibot.AddLogEntryIfNotContained(updateLogsRequest),
            updateStatesRequest => dezibot.AddClassStatesIfNotContained(updateStatesRequest));

        await dbContext.SaveChangesAsync();
        
        // TODO: Dont sent the entire dezibot object, only the updated parts. Maybe use some kind of cache.
        await hubContext.Clients.All.SendDezibotUpdateAsync(dezibot);
        return Results.NoContent();
    }

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
        
        if (updateLogsRequest?.Ip is not null && updateLogsRequest?.ClassName/*.TimestampUtc*/ is not null)
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

        if (updateStatesRequest?.Ip is not null && updateStatesRequest?.Data/*.TimestampUtc*/ is not null)
        {
            return updateStatesRequest;
        }

        return null;
    }

    private static void AddLogEntryIfNotContained(this Dezibot dezibot, UpdateDezibotLogsRequest request)
    {
        var logEntry = new Dezibot.LogEntry(
            dezibot.LastConnectionUtc,/*request.TimestampUtc,*/
            request.ClassName,
            request.Message,
            request.Data);

        if (!dezibot.Logs.Contains(logEntry))
        {
            dezibot.Logs.Add(logEntry);
        }
    }

    private static void AddClassStatesIfNotContained(this Dezibot dezibot, UpdateDezibotStatesRequest request)
    {
        var newClasses = request.Data.Select(state => new Dezibot.Class(
            name: state.Key,
            properties: state.Value.Select(property => new Dezibot.Class.Property(
                name: property.Key,
                values: [new Dezibot.Class.Property.TimeValue(dezibot.LastConnectionUtc,/*request.TimestampUtc,*/ property.Value)]
            )).ToList()
        )).ToList();

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

                var newTimeValues = newProperty.Values
                    .Where(timeValue => !existingProperty.Values.Contains(timeValue))
                    .ToList();

                existingProperty.Values.AddRange(newTimeValues);
            }
        }
    }
}