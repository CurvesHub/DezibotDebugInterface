using DezibotDebugInterface.Api.Endpoints.Common;

namespace DezibotDebugInterface.Api.Endpoints.SignalR;

/// <summary>
/// Represents a client of the DezibotHub.
/// </summary>
public interface IDezibotHubClient
{
    /// <summary>
    /// Sends the latest dezibot to the client.
    /// </summary>
    /// <param name="dezibot">The latest dezibot.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DezibotUpdated(DezibotViewModel dezibot);
}