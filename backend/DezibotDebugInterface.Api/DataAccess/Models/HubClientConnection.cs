using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a client connection to the hub.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "The string fields wont be longer than 255 characters.")]
public class HubClientConnection
{
    /// <summary>
    /// The unique identifier of the client connection.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// The connection ID of the client.
    /// </summary>
    public string ConnectionId { get; init; }
    
    /// <summary>
    /// Whether the client wants the session to continue being updated.
    /// </summary>
    public bool ContinueSession { get; init; }
    
    /// <summary>
    /// Foreign key to the session.
    /// </summary>
    public int SessionId { get; init; }

    /// <summary>
    /// Creates a new instance of <see cref="HubClientConnection"/>.
    /// </summary>
    /// <param name="connectionId">The connection ID of the client.</param>
    /// <param name="continueSession">The client's preference for session updates.</param>
    public HubClientConnection(string connectionId, bool continueSession)
    {
        ConnectionId = connectionId;
        ContinueSession = continueSession;
    }

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    [UsedImplicitly]
#pragma warning disable CS8618, CS9264
    public HubClientConnection() { }
#pragma warning restore CS8618, CS9264
}