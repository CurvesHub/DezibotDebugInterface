namespace DezibotDebugInterface.Api.Endpoints.Models;

/// <summary>
/// Represents a log of a component.
/// </summary>
/// <param name="Message">The message of the log.</param>
/// <param name="Values">The values of the log.</param>
public record Log(
    string Message,
    IEnumerable<DebugValue> Values);