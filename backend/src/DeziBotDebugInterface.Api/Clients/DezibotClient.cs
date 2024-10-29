using DeziBotDebugInterface.Api.Endpoints.Requests;
using ErrorOr;

namespace DeziBotDebugInterface.Api.Clients;

/// <inheritdoc />
public class DezibotClient : IDezibotClient // TODO: Implement missing methods
{
    /// <inheritdoc />
    public Task<ErrorOr<Success>> ReceiveBroadcastAsync(ReceiveBroadcastRequest request)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ErrorOr<Success>> SendCommandAsync(SendCommandRequest request)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ErrorOr<Success>> SendCommandByIdAsync(SendCommandRequest request, string serialNumber)
    {
        throw new NotImplementedException();
    }
}