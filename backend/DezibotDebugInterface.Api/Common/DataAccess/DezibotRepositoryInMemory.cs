using DezibotDebugInterface.Api.Common.Models;

namespace DezibotDebugInterface.Api.Common.DataAccess;

/// <inheritdoc />
public class DezibotRepositoryInMemory : IDezibotRepository
{
    private readonly List<Dezibot> _dezibots = [];
    
    /// <inheritdoc />
    public IAsyncEnumerable<Dezibot> GetAllDezibotsAsync()
    {
        return _dezibots.ToAsyncEnumerable();
    }

    /// <inheritdoc />
    public Task<Dezibot?> GetByIpAsync(string ip)
    {
        return Task.FromResult(_dezibots.FirstOrDefault(dezibot => dezibot.Ip == ip));
    }

    /// <inheritdoc />
    public Task<Dezibot> UpdateAsync(string ip, List<Dezibot.Debuggable>? debuggables = null, List<Dezibot.LogEntry>? logs = null)
    {
        var newDezibot = new Dezibot(ip, DateTime.UtcNow, debuggables, logs);
        
        var existingDezibot = _dezibots.FirstOrDefault(dezibot => dezibot.Ip == ip);

        if (existingDezibot is null)
        {
            _dezibots.Add(newDezibot);
        }
        else
        {
            Dezibot.Update(existingDezibot, newDezibot);
        }

        return Task.FromResult(existingDezibot ?? newDezibot);
    }
}