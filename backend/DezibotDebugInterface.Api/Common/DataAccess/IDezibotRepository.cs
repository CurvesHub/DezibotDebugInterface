using DezibotDebugInterface.Api.Common.Models;

namespace DezibotDebugInterface.Api.Common.DataAccess;

/// <summary>
/// Provides access to Dezibot data.
/// </summary>
public interface IDezibotRepository
{
    /// <summary>
    /// Get all Dezibots.
    /// </summary>
    /// <returns>A <see cref="IAsyncEnumerable{T}"/> of <see cref="Dezibot"/>.</returns>
    IAsyncEnumerable<Dezibot> GetAllDezibotsAsync();
    
    /// <summary>
    /// Get a Dezibot by IP.
    /// </summary>
    /// <param name="ip">The IP of the Dezibot to get.</param>
    /// <returns>A <see cref="Dezibot"/> or <see langword="null"/> if no Dezibot was found.</returns>
    Task<Dezibot?> GetByIpAsync(string ip);
    
    /// <summary>
    /// Update or add a Dezibot.
    /// </summary>
    /// <param name="ip">The IP of the Dezibot to update or add.</param>
    /// <param name="debuggables">The debuggables to update or add.</param>
    /// <param name="logs">The logs to update or add.</param>
    /// <returns>The updated or added <see cref="Dezibot"/>.</returns>
    Task<Dezibot> UpdateAsync(string ip, List<Dezibot.Debuggable>? debuggables = null, List<Dezibot.LogEntry>? logs = null);
}