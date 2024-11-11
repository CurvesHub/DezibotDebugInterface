namespace DezibotDebugInterface.Api.Endpoints.Models;

/// <summary>
/// Represents a component of a Dezibot.
/// </summary>
/// <param name="Name">The name of the component.</param>
/// <param name="Properties">The properties of the component.</param>
/// <param name="Logs">The logs of the component.</param>
public record Component(
    string Name,
    IEnumerable<Property> Properties,
    IEnumerable<Log> Logs);