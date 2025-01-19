using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a Dezibot.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "The string fields wont be longer than 255 characters.")]
public class Dezibot
{
    /// <summary>
    /// The unique identifier of the Dezibot.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// The session identifier of the Dezibot.
    /// </summary>
    public int? SessionId { get; init; }
    
    /// <summary>
    /// Gets the IP address of the Dezibot, which uniquely identifies it.
    /// </summary>
    public string Ip { get; init; }
    
    /// <summary>
    /// Gets or sets the last connection time of the Dezibot in UTC.
    /// </summary>
    public DateTimeOffset LastConnectionUtc { get; set; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// Gets the logs of the Dezibot.
    /// </summary>
    public List<LogEntry> Logs { get; init; } = [];

    /// <summary>
    /// Get the classes of the Dezibot.
    /// </summary>
    public List<Class> Classes { get; init; } = [];

    /// <summary>
    /// Creates a new instance of the <see cref="Dezibot"/> class.
    /// </summary>
    /// <param name="ip">The IP address of the Dezibot.</param>
    /// <param name="sessionId">The session identifier of the Dezibot.</param>
    public Dezibot(string ip, int? sessionId = null)
    {
        Ip = ip;
        SessionId = sessionId;
    }
    
    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    [UsedImplicitly]
#pragma warning disable CS8618, CS9264
    private Dezibot() { }
#pragma warning restore CS8618, CS9264
}