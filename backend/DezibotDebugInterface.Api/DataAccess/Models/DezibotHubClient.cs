using System.Diagnostics.CodeAnalysis;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a client connected to the hub.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "The string fields wont be longer than 255 characters.")]
public class DezibotHubClient
{
    /// <summary>
    /// The unique identifier of the client connection.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// The connection ID of the client.
    /// </summary>
    public required string ConnectionId { get; init; }
    
    /// <summary>
    /// The session client connections of the client.
    /// </summary>
    public List<SessionClientConnection> Sessions { get; init; } = [];
}