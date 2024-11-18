using Microsoft.AspNetCore.SignalR;

namespace DezibotDebugInterface.Api.Broadcast.DezibotHubs;

/// <summary>
/// The SignalR hub for sending updates about dezibots.
/// </summary>
public sealed class DezibotHub : Hub<IDezibotHubClient>;