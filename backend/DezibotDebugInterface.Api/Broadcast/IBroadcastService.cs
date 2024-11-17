using DezibotDebugInterface.Api.Broadcast.Models;

namespace DezibotDebugInterface.Api.Broadcast;

/// <summary>
/// Handles the received broadcast data from the dezibots.
/// </summary>
public interface IBroadcastService
{
    /// <summary>
    /// Handles the received state broadcast data.
    /// </summary>
    /// <param name="request">The state broadcast request to handle.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleStateBroadcastDataAsync(StateBroadcastRequest request);
    
    /// <summary>
    /// Handles the received log broadcast data.
    /// </summary>
    /// <param name="request">The log broadcast request to handle.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task HandleLogBroadcastDataAsync(LogBroadcastRequest request);
}