using System.Globalization;

using DezibotDebugInterface.Api.Broadcast.DezibotHubs;
using DezibotDebugInterface.Api.Broadcast.Models;
using DezibotDebugInterface.Api.Common.DataAccess;
using DezibotDebugInterface.Api.Common.Models;

using Microsoft.AspNetCore.SignalR;

namespace DezibotDebugInterface.Api.Broadcast;

/// <inheritdoc />
public class BroadcastService(
    IDezibotRepository dezibotRepository,
    IHubContext<DezibotHub, IDezibotHubClient> hubContext)
    : IBroadcastService
{
    /// <inheritdoc />
    public async Task HandleStateBroadcastDataAsync(StateBroadcastRequest request)
    {
        var dezibot = await dezibotRepository.GetByIpAsync(request.Ip);

        if (dezibot is null)
        {
            dezibot = new Dezibot(request.Ip, DateTime.UtcNow, request.Debuggables);
        }
        else
        {
            dezibot.Debuggables.AddRange(request.Debuggables);
        }
        
        await dezibotRepository.UpdateAsync(dezibot);
        await NotifyDezibotClientsAsync(dezibot);
    }

    /// <inheritdoc />
    public async Task HandleLogBroadcastDataAsync(LogBroadcastRequest request)
    {
        var dezibot = await dezibotRepository.GetByIpAsync(request.Ip);

        if (dezibot is null)
        {
            dezibot = new Dezibot(request.Ip, DateTime.UtcNow, logs: [CreateLogEntriesFromStrings(request)]);
        }
        else
        {
            dezibot.Logs.Add(CreateLogEntriesFromStrings(request));
        }
        
        await dezibotRepository.UpdateAsync(dezibot);
        await NotifyDezibotClientsAsync(dezibot);
    }

    private static Dezibot.LogEntry CreateLogEntriesFromStrings(LogBroadcastRequest request)
    {
        return new Dezibot.LogEntry(
            DateTime.Parse(request.TimestampUtc, CultureInfo.InvariantCulture),
            request.LogLevel,
            request.Message);
    }
    
    private async Task NotifyDezibotClientsAsync(Dezibot dezibot)
    {
        await hubContext.Clients.All.SendDezibotUpdateAsync(dezibot);
    }
}