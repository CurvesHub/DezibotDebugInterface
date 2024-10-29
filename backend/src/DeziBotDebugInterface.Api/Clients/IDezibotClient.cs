using DeziBotDebugInterface.Api.Endpoints.Requests;
using ErrorOr;

namespace DeziBotDebugInterface.Api.Clients;

/// <summary>
/// Interacts with a Dezibot via HTTP.
/// </summary>
public interface IDezibotClient
{
    /// <summary>
    /// Receives a broadcast from a Dezibot.
    /// </summary>
    /// <param name="request">The request data for the broadcast.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> containing a success value if the broadcast was received successfully, or an error if not.</returns>
    Task<ErrorOr<Success>> ReceiveBroadcastAsync(ReceiveBroadcastRequest request);

    /// <summary>
    /// Sends a command to a Dezibot.
    /// </summary>
    /// <param name="request">The request data for the command.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> containing a success value if the command was sent successfully, or an error if not.</returns>
    Task<ErrorOr<Success>> SendCommandAsync(SendCommandRequest request);

    /// <summary>
    /// Sends a command to a Dezibot by its serial number.
    /// </summary>
    /// <param name="request">The request data for the command.</param>
    /// <param name="serialNumber">The serial number of the Dezibot to send the command to.</param>
    /// <returns>An <see cref="ErrorOr{TValue}"/> containing a success value if the command was sent successfully, or an error if not.</returns>
    Task<ErrorOr<Success>> SendCommandByIdAsync(SendCommandRequest request, string serialNumber);
}