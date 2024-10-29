using DeziBotDebugInterface.Api.Endpoints.Requests;
using DeziBotDebugInterface.Api.Models;
using ErrorOr;

namespace DeziBotDebugInterface.Api.Repositories;

/// <inheritdoc />
public class DezibotRepository : IDezibotRepository // TODO: Implement missing methods
{
    /// <inheritdoc />
    public IAsyncEnumerable<Dezibot> GetDezibotsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<Dezibot?> GetDezibotByIdAsync(string serialNumber)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ErrorOr<string>> AddDezibotAsync(AddDezibotRequest request)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<ErrorOr<Success>> ReceiveBroadcastAsync(Broadcast broadcast)
    {
        throw new NotImplementedException();
    }
}