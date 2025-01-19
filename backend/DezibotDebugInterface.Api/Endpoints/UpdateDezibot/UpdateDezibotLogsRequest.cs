using DezibotDebugInterface.Api.DataAccess.Models;

using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Endpoints.UpdateDezibot;

/// <summary>
/// Represents a request to update the logs of a dezibot.
/// </summary>
/// <param name="Ip">The IP address of the dezibot.</param>
/// <param name="LogLevel">The log level of the log.</param>
/// <param name="ClassName">The class name where the log message originated.</param>
/// <param name="Message">The message of the log.</param>
/// <param name="Data">Additional data of the log.</param>
/// <example>
/// <code>
/// {
///     "Ip": "111.222.333.444",
///     "logLevel": "INFO",
///     "className": "DISPLAY",
///     "message": "My first message",
///     "data": "Some data"
/// }
/// </code>
/// </example>
[PublicAPI]
public record UpdateDezibotLogsRequest(
    string Ip,
    DezibotLogLevel LogLevel,
    string ClassName,
    string Message,
    string? Data);