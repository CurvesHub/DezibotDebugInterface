namespace DezibotDebugInterface.Api.Endpoints.Models;

/// <summary>
/// Represents a debug value.
/// </summary>
/// <param name="TimestampUtc">The time associated with the value.</param>
/// <param name="Value">The debug value.</param>
public record DebugValue(
    string TimestampUtc,
    string Value);