namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a Dezibot.
/// </summary>
public class Dezibot
{
    /// <summary>
    /// The unique identifier of the Dezibot.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// Gets the IP address of the Dezibot, which uniquely identifies it.
    /// </summary>
    public string Ip { get; init; }
    
    /// <summary>
    /// Gets or sets the last connection time of the Dezibot in UTC.
    /// </summary>
    public DateTimeOffset LastConnectionUtc { get; set; }
    
    /// <summary>
    /// Gets the logs of the Dezibot.
    /// </summary>
    public List<LogEntry> Logs { get; init; }
    
    /// <summary>
    /// Get the classes of the Dezibot.
    /// </summary>
    public List<Class> Classes { get; init; }
    
    /// <summary>
    /// Creates a new instance of the <see cref="Dezibot"/> class.
    /// </summary>
    /// <param name="ip">The IP address of the Dezibot.</param>
    /// <param name="lastConnectionUtc">The last connection time of the Dezibot in UTC.</param>
    /// <param name="logs">The logs of the Dezibot.</param>
    /// <param name="classes">The classes of the Dezibot.</param>
    public Dezibot(
        string ip,
        DateTimeOffset lastConnectionUtc,
        List<LogEntry>? logs = null,
        List<Class>? classes = null)
    {
        Ip = ip;
        LastConnectionUtc = lastConnectionUtc;
        Logs = logs ?? [];
        Classes = classes ?? [];
    }
    
    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    private Dezibot() { }
}