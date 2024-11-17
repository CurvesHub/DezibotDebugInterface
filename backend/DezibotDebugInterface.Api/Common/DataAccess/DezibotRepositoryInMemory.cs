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
    public Task UpdateAsync(Dezibot dezibot)
    {
        var existingDezibot = _dezibots.FirstOrDefault(d => d.Ip == dezibot.Ip);

        if (existingDezibot is null)
        {
            _dezibots.Add(dezibot);
        }
        else
        {
            UpdateDeziBot(existingDezibot, dezibot);
        }

        return Task.CompletedTask;
    }
    
    private static void UpdateDeziBot(Dezibot existingDezibot, Dezibot newDezibot)
    {
        existingDezibot.LastConnectionUtc = newDezibot.LastConnectionUtc;

        foreach (var newDebuggable in newDezibot.Debuggables)
        {
            var existingDebuggable = existingDezibot.Debuggables.FirstOrDefault(debuggable => debuggable.Name == newDebuggable.Name);
            
            if (existingDebuggable is null)
            {
                existingDezibot.Debuggables.Add(newDebuggable);
                continue;
            }

            foreach (var newProperty in newDebuggable.Properties)
            {
                var existingProperty = existingDebuggable.Properties.FirstOrDefault(property => property.Name == newProperty.Name);

                if (existingProperty is null)
                {
                    existingDebuggable.Properties.Add(newProperty);
                    continue;
                }

                var newTimeValues = newProperty.Values.Where(timeValue => !existingProperty.Values.Contains(timeValue));
                existingProperty.Values.AddRange(newTimeValues);
            }
        }
        
        var newLogEntries = newDezibot.Logs.Where(logEntry => !existingDezibot.Logs.Contains(logEntry));
        existingDezibot.Logs.AddRange(newLogEntries);
    }
}