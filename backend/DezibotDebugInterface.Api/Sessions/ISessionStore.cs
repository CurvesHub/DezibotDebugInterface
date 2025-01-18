using DezibotDebugInterface.Api.DataAccess.Models;

namespace DezibotDebugInterface.Api.Sessions;

/// <summary>
/// Manages the active sessions.
/// </summary>
public interface ISessionStore
{
    /// <summary>
    /// Creates a new active session.
    /// </summary>
    /// <param name="connectionId">The connection ID of the client.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task CreateActiveSessionAsync(string connectionId);
    
    /// <summary>
    /// Deactivates the session.
    /// </summary>
    /// <param name="connectionId">The connection ID of the client.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task DeactivateSessionAsync(string connectionId);
    
    /// <summary>
    /// Retrieves the active session.
    /// </summary>
    /// <returns>A read-only list of active sessions.</returns>
    public Task<IReadOnlyList<Session>> GetActiveSessionAsync();
    
    /// <summary>
    /// Retrieves all sessions.
    /// </summary>
    /// <returns>A read-only list of all sessions.</returns>
    public Task<IReadOnlyList<Session>> GetAllSessionsAsync();
}