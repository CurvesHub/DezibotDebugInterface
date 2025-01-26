namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a connection between a session and a client.
/// </summary>
public class SessionClientConnection
{
    /// <summary>
    /// The ID of the connection.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// Indicates whether the client wants to receive updates about the session.
    /// </summary>
    public bool ReceiveUpdates { get; set; } = true;
    
    /// <summary>
    /// The ID of the session.
    /// </summary>
    public int SessionId { get; init; }
    
    /// <summary>
    /// The session.
    /// </summary>
    public Session? Session { get; init; }

    /// <summary>
    /// The ID of the client.
    /// </summary>
    public int ClientId { get; init; }
    
    /// <summary>
    /// The client.
    /// </summary>
    public DezibotHubClient? Client { get; init; }
}