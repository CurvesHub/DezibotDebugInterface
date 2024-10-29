using DeziBotDebugInterface.Api.Endpoints.Requests;
using DeziBotDebugInterface.Api.Models;
using ErrorOr;

namespace DeziBotDebugInterface.Api.Repositories;

/// <summary>
/// Provides access to the Dezibot data store.
/// </summary>
public interface IDezibotRepository
{
    /// <summary>
    /// Gets all Dezibots.
    /// </summary>
    /// <returns>An async enumerable of Dezibots.</returns>
    IAsyncEnumerable<Dezibot> GetDezibotsAsync();
    
    /// <summary>
    /// Gets a Dezibot by its serial number.
    /// </summary>
    /// <param name="serialNumber">The serial number of the Dezibot to get.</param>
    /// <returns>The Dezibot with the specified serial number, or null if no such Dezibot exists.</returns>
    Task<Dezibot?> GetDezibotByIdAsync(string serialNumber);
    
    /// <summary>
    /// Adds a new Dezibot.
    /// </summary>
    /// <param name="request">The request to add a new Dezibot.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> containing the serial number of the new Dezibot if successful, or an error if not.</returns>
    Task<ErrorOr<string>> AddDezibotAsync(AddDezibotRequest request);
    
    /// <summary>
    /// Saves the received broadcast.
    /// </summary>
    /// <param name="broadcast">The broadcast to save.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> containing the success status of the operation.</returns>
    Task<ErrorOr<Success>> ReceiveBroadcastAsync(Broadcast broadcast);
}