using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Endpoints.UpdateDezibot;

/// <summary>
/// Represents a request to update the states of a dezibot.
/// </summary>
/// <param name="Ip">The IP address of the dezibot.</param>
/// <param name="Data">The state data of the dezibot.</param>
/// <example>
/// <code>
/// {
///     "Ip": "111.222.333.444",
///     "Data": {
///         "className": {
///             "propertyName1": "value1",
///             "propertyName2": "value2"
///         },
///         "DISPLAY": {
///             "isFlipped": "true",
///             "currentLine": "12"
///         }
///     }
/// }
/// </code>
/// </example>
[PublicAPI]
public record UpdateDezibotStatesRequest(
    string Ip,
    Dictionary<string, Dictionary<string, string>> Data);