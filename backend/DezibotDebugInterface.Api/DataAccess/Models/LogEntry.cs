namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a log entry.
/// </summary>
public class LogEntry
{
    /// <summary>
    /// The unique identifier of the log entry.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// Foreign key to the <see cref="Dezibot"/> entity.
    /// </summary>
    public int DezibotId { get; init; }
    
    /// <summary>
    /// The timestamp of the log message in UTC.
    /// </summary>
    public DateTimeOffset TimestampUtc { get; init; }

    /// <summary>
    /// The class name where the log message originated.
    /// </summary>
    public string ClassName { get; init; }

    /// <summary>
    /// The message of the log.
    /// </summary>
    public string Message { get; init; }

    /// <summary>
    /// Additional data of the log.
    /// </summary>
    public string? Data { get; init; }
    
    /// <summary>
    /// Creates a new instance of the <see cref="LogEntry"/> class.
    /// </summary>
    /// <param name="timestampUtc">The timestamp of the log message in UTC.</param>
    /// <param name="className">The class name where the log message originated.</param>
    /// <param name="message">The message of the log.</param>
    /// <param name="data">Additional data of the log.</param>
    public LogEntry(
        DateTimeOffset timestampUtc,
        string className,
        string message,
        string? data)
    {
        TimestampUtc = timestampUtc;
        ClassName = className;
        Message = message;
        Data = data;
    }
    
    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    private LogEntry() { }
}