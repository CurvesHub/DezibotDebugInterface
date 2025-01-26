using System.Diagnostics.CodeAnalysis;

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
    /// The dezibots associated with this session.
    /// </summary>
    public List<Dezibot> Dezibots { get; init; } = [];
    
    /// <summary>
    /// The session client connections of the session.
    /// </summary>
    public List<SessionClientConnection> SessionClientConnections { get; init; } = [];
}