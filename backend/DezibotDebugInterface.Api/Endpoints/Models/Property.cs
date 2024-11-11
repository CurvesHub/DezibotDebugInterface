namespace DezibotDebugInterface.Api.Endpoints.Models;

/// <summary>
/// Represents a property of a component.
/// </summary>
/// <param name="Name">The name of the property.</param>
/// <param name="Values">The values of the property.</param>
public record Property(
    string Name,
    IEnumerable<DebugValue> Values);