using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a session.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "The string fields wont be longer than 255 characters.")]
public class Session
{
    /// <summary>
    /// The unique identifier of the session.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// Indicates whether the session is active.
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// The date and time the session was created in UTC.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// The SignalR connection ID for this session.
    /// </summary>
    public string ClientConnectionId { get; init; }
    
    /// <summary>
    /// The dezibots associated with this session.
    /// </summary>
    public List<Dezibot> Dezibots { get; init; } = [];
    
    /// <summary>
    /// Creates a new instance of the <see cref="Session"/> class.
    /// </summary>
    /// <param name="clientConnectionId">The SignalR connection in the session.</param>
    public Session(string clientConnectionId)
    {
        ClientConnectionId = clientConnectionId;
    }
    
    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    [UsedImplicitly]
#pragma warning disable CS8618, CS9264
    private Session() { }
#pragma warning restore CS8618, CS9264
}