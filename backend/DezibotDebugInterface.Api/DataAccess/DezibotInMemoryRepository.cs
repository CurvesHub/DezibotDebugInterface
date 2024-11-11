using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Endpoints.Models;

namespace DezibotDebugInterface.Api.DataAccess;

/// <inheritdoc />
public class DezibotInMemoryRepository : IDezibotRepository
{
    private readonly List<Dezibot> _dezibots = [];
    
    /// <inheritdoc />
    public IAsyncEnumerable<Dezibot> GetAllDezibotsAsync()
    {
        return _dezibots.ToAsyncEnumerable();
    }

    /// <inheritdoc />
    public Task<Dezibot?> GetDezibotByIpAsync(string ip)
    {
        return Task.FromResult(_dezibots.FirstOrDefault(dezibot => dezibot.Ip == ip));
    }

    /// <inheritdoc />
    public Task<bool> SaveBroadcastDataAsync(PutDezibotRequest request)
    {
        var dezibotToUpdate = _dezibots.FirstOrDefault(dezibot => dezibot.Ip == request.Ip);

        if (dezibotToUpdate is null)
        {
            var dezibot = Dezibot.FromPutRequest(request);
            _dezibots.Add(dezibot);
        }
        else
        {
            var updatedDezibot = Dezibot.UpdateDezibotFromPutRequest(dezibotToUpdate, request);
            _dezibots[_dezibots.IndexOf(dezibotToUpdate)] = updatedDezibot;
        }

        return Task.FromResult(true);
    }
}

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
    Task<Dezibot?> GetDezibotByIpAsync(string ip);

    /// <summary>
    /// Save broadcast data.
    /// </summary>
    /// <param name="request">The request with the data to save.</param>
    /// <returns>A boolean indicating whether the save was successful.</returns>
    Task<bool> SaveBroadcastDataAsync(PutDezibotRequest request);
}