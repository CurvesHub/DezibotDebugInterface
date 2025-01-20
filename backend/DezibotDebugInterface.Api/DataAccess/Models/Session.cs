using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a session.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "The string fields wont be longer than 255 characters.")]
[SuppressMessage("SonarQube", "S3427: Method overloads with default parameter values should not overlap", Justification = "EF Core requires parameterless constructor.")]
public class Session
{
    /// <summary>
    /// The unique identifier of the session.
    /// </summary>
    public int Id { get; init; }
 
    /// <summary>
    /// The name of the session.
    /// </summary>
    public string? Name { get; init; }
    
    /// <summary>
    /// The date and time the session was created in UTC.
    /// </summary>
    public DateTimeOffset CreatedUtc { get; init; } = DateTimeOffset.UtcNow;
    
    /// <summary>
    /// The SignalR client connection IDs for this session.
    /// </summary>
    public List<HubClientConnection> ClientConnections { get; init; } = [];
    
    /// <summary>
    /// The dezibots associated with this session.
    /// </summary>
    public List<Dezibot> Dezibots { get; init; } = [];
    
    /// <summary>
    /// Creates a new instance of the <see cref="Session"/> class.
    /// </summary>
    /// <param name="name">The name of the session.</param>
    public Session(string? name = null)
    {
        Name = name;
    }
    
    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    [UsedImplicitly]
#pragma warning disable CS8618, CS9264
    private Session() { }
#pragma warning restore CS8618, CS9264
}