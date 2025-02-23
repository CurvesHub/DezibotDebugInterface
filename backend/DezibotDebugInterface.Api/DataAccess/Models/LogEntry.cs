using System.Diagnostics.CodeAnalysis;

using DezibotDebugInterface.Api.DataAccess.Models.Enums;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a log entry.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "The string fields wont be longer than 255 characters.")]
public class LogEntry
{
    /// <summary>
    /// The unique identifier of the log entry.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// The timestamp of the log message in UTC.
    /// </summary>
    public required DateTimeOffset TimestampUtc { get; init; }

    /// <summary>
    /// The log level of the log message.
    /// </summary>
    public required DezibotLogLevel LogLevel { get; init; }

    /// <summary>
    /// The class name where the log message originated.
    /// </summary>
    public required string ClassName { get; init; }

    /// <summary>
    /// The message of the log.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Additional data of the log.
    /// </summary>
    public string? Data { get; init; }
        
    /// <summary>
    /// Foreign key to the <see cref="Dezibot"/> entity.
    /// </summary>
    public int? DezibotId { get; init; }
}