using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Endpoints.UpdateDezibot;

/// <summary>
/// Represents a request to update the logs of a dezibot.
/// </summary>
/// <param name="Ip">The IP address of the dezibot.</param>
/// <param name="TimestampUtc">The timestamp of the log message in UTC.</param>
/// <param name="ClassName">The class name where the log message originated.</param>
/// <param name="Message">The message of the log.</param>
/// <param name="Data">Additional data of the log.</param>
[PublicAPI]
public record UpdateDezibotLogsRequest(
    string Ip,
    DateTime TimestampUtc,
    string ClassName,
    string Message,
    string? Data);
