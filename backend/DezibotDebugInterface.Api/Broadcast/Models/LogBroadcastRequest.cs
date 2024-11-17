using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Broadcast.Models;

/// <summary>
/// The request to broadcast a log message.
/// </summary>
/// <param name="TimestampUtc">The timestamp of the log message in UTC.</param>
/// <param name="LogLevel">The log level of the message.</param>
/// <param name="Message">The message to log.</param>
[PublicAPI]
public record LogBroadcastRequest(string Ip, string TimestampUtc, string LogLevel, string Message);