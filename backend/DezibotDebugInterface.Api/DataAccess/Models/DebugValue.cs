using System.Globalization;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a debug value.
/// </summary>
/// <param name="TimestampUtc">The time associated with the value.</param>
/// <param name="Value">The debug value.</param>
public record DebugValue(
    DateTime TimestampUtc,
    string Value)
{
    /// <summary>
    /// Maps a <see cref="Endpoints.Models.DebugValue"/> to a <see cref="DebugValue"/>.
    /// </summary>
    /// <param name="debugValue">The debug value to map.</param>
    /// <returns>The mapped <see cref="DebugValue"/>.</returns>
    public static DebugValue FromDebugValue(Endpoints.Models.DebugValue debugValue)
    {
        return new DebugValue(
            TimestampUtc: DateTime.Parse(debugValue.TimestampUtc, CultureInfo.InvariantCulture),
            Value: debugValue.Value);
    }
}