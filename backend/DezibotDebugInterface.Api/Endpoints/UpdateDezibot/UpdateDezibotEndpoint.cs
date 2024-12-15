using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Endpoints.Constants;
using DezibotDebugInterface.Api.Endpoints.GetDezibots;
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
        endpoints.MapPut("api/dezibot/update", UpdateDezibotAsync)
            .WithName("Update Dezibot")
            .WithSummary("Accepts state and log data from a dezibot and updates the database.")
            .Accepts<UpdateDezibotLogsRequest>(ContentTypes.JsonContentType)
            .Accepts<UpdateDezibotStatesRequest>(ContentTypes.JsonContentType)
            .Produces((int)HttpStatusCode.NoContent)
            .ProducesProblem((int)HttpStatusCode.BadRequest, ContentTypes.ProblemContentType)
            .ProducesProblem((int)HttpStatusCode.InternalServerError, ContentTypes.ProblemContentType)
            .WithOpenApi();
        
        return endpoints;
    }
    
    private static async Task<IResult> UpdateDezibotAsync(
        HttpContext httpContext,
        DezibotDbContext dbContext,
        IHubContext<DezibotHub, IDezibotHubClient> hubContext)
    {
        // TODO: Session Handling - Check if the request is part of an existing session. Otherwise store it without a session or dump it?
        // Implement a session model and management and migrate the database
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

        var dezibot = await dbContext.Dezibots.Where(dezibot => dezibot.Ip == ip).FirstOrDefaultAsync();
        if (dezibot is null)
        {
            dezibot = new Dezibot(ip, DateTimeOffset.UtcNow);
            await dbContext.AddAsync(dezibot);
        }
        
        dezibot.LastConnectionUtc = DateTimeOffset.UtcNow;

        request.Value.Switch(
            updateLogsRequest => dezibot.Logs.Add(new LogEntry(dezibot.LastConnectionUtc, updateLogsRequest.ClassName, updateLogsRequest.Message, updateLogsRequest.Data)),
            updateStatesRequest => dezibot.UpdateClassStates(updateStatesRequest));

        await dbContext.SaveChangesAsync();

        await hubContext.Clients.All.SendDezibotUpdateAsync(dezibot.ToDezibotViewModel());
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