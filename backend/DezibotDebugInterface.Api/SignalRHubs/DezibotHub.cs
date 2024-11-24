using Microsoft.AspNetCore.SignalR;

namespace DezibotDebugInterface.Api.SignalRHubs;

/// <summary>
/// The SignalR hub for sending updates about dezibots.
/// </summary>
public sealed class DezibotHub : Hub<IDezibotHubClient>;
// TODO: Override OnConnectedAsync, OnDisconnectedAsync, and OnReconnectedAsync to handle connection events.
// We want to implement some kind of session management therefore a connect could start a session and a disconnect could end it.
