namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a log of a component.
/// </summary>
/// <param name="Message">The message of the log.</param>
/// <param name="Values">The values of the log.</param>
public record Log(
    string Message,
    List<DebugValue> Values)
{
    /// <summary>
    /// Maps a <see cref="Endpoints.Models.Log"/> to a <see cref="Log"/>.
    /// </summary>
    /// <param name="log">The log to map.</param>
    /// <returns>The mapped <see cref="Log"/>.</returns>
    public static Log FromLog(Endpoints.Models.Log log)
    {
        return new Log(
            Message: log.Message,
            Values: log.Values.Select(DebugValue.FromDebugValue).ToList());
    }
}