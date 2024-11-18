using DezibotDebugInterface.Api.Common.Models;

namespace DezibotDebugInterface.Api.Broadcast.DezibotHubs;

/// <summary>
/// Represents a client that can receive updates about a dezibot.
/// </summary>
public interface IDezibotHubClient
{
    /// <summary>
    /// Sends an update about a dezibot.
    /// </summary>
    /// <param name="dezibot">The dezibot to send an update about.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task SendDezibotUpdateAsync(Dezibot dezibot);
}