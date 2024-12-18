using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents the log level of a log message.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "The names are consistent with the names used by the dezibot and frontend.")]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DezibotLogLevel
{
    /// <summary>
    /// The log message is a debug message.
    /// </summary>
    DEBUG,
    
    /// <summary>
    /// The log message is an info message.
    /// </summary>
    INFO,
    
    /// <summary>
    /// The log message is a warning message.
    /// </summary>
    WARN,
    
    /// <summary>
    /// The log message is an error message.
    /// </summary>
    ERROR,
    
    /// <summary>
    /// The log message has an unknown log level.
    /// </summary>
    UNKNOWN
}