using System.Globalization;

using DezibotDebugInterface.Api.Endpoints.Models;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a Dezibot.
/// </summary>
/// <param name="Ip">The ip address of the Dezibot, which is a unique identifier.</param>
/// <param name="LastConnectionUtc">The last time the Dezibot connected to the server.</param>
/// <param name="Components">The components of the Dezibot.</param>
public record Dezibot(
    string Ip,
    DateTime LastConnectionUtc,
    List<Component> Components)
{
    /// <summary>
    /// Creates a <see cref="Dezibot"/> from a <see cref="PutDezibotRequest"/>.
    /// </summary>
    /// <param name="request">The request to create the Dezibot from.</param>
    /// <returns>A new instance of <see cref="Dezibot"/>.</returns>
    public static Dezibot FromPutRequest(PutDezibotRequest request)
    {
        return new Dezibot(
            Ip: request.Ip,
            LastConnectionUtc: DateTime.Parse(request.LastConnectionUtc, CultureInfo.InvariantCulture),
            Components: request.Components.Select(Component.FromComponent).ToList());
    }

    /// <summary>
    /// Updates a <see cref="Dezibot"/> from a <see cref="PutDezibotRequest"/>.
    /// </summary>
    /// <param name="dezibot">The Dezibot to update.</param>
    /// <param name="request">The request to update the Dezibot from.</param>
    /// <returns>A new instance of <see cref="Dezibot"/> with the updated components.</returns>
    public static Dezibot UpdateDezibotFromPutRequest(Dezibot dezibot, PutDezibotRequest request)
    {
        Dictionary<string, Component> updatedComponents = dezibot.Components.ToDictionary(component => component.Name);

        foreach (var component in request.Components)
        {
            if (updatedComponents.TryGetValue(component.Name, out var existingComponent))
            {
                UpdateProperties(component, existingComponent);
                UpdateLogs(component, existingComponent);
            }
            else
            {
                updatedComponents[component.Name] = Component.FromComponent(component);
            }
        }

        return dezibot with
        {
            LastConnectionUtc = DateTime.Parse(request.LastConnectionUtc, CultureInfo.InvariantCulture),
            Components = updatedComponents.Values.ToList()
        };
    }

    private static void UpdateProperties(Endpoints.Models.Component component, Component existingComponent)
    {
        foreach (var property in component.Properties)
        {
            var existingProperty = existingComponent.Properties.FirstOrDefault(p => p.Name == property.Name);
            if (existingProperty is null)
            {
                existingComponent.Properties.Add(Property.FromProperty(property));
            }
            else
            {
                var youngestTimestampUtc = existingProperty.Values.Max(v => v.TimestampUtc);
                
                var newValues = property.Values
                    .Select(DebugValue.FromDebugValue)
                    .Where(debugValue => debugValue.TimestampUtc > youngestTimestampUtc);

                existingProperty.Values.AddRange(newValues);
            }
        }
    }
    
    private static void UpdateLogs(Endpoints.Models.Component component, Component existingComponent)
    {
        foreach (var log in component.Logs)
        {
            var existingLog = existingComponent.Logs.FirstOrDefault(l => l.Message == log.Message);
            if (existingLog is null)
            {
                existingComponent.Logs.Add(Log.FromLog(log));
            }
            else
            {
                var youngestTimestampUtc = existingLog.Values.Max(v => v.TimestampUtc);

                var newValues = log.Values
                    .Select(DebugValue.FromDebugValue)
                    .Where(debugValue => debugValue.TimestampUtc > youngestTimestampUtc);
                
                existingLog.Values.AddRange(newValues);
            }
        }
    }
}