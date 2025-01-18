using DezibotDebugInterface.Api.Sessions;

using Microsoft.AspNetCore.SignalR;

namespace DezibotDebugInterface.Api.SignalRHubs;

/// <summary>
/// The SignalR hub for sending updates about dezibots.
/// </summary>
/// <param name="sessionStore">The session store.</param>
public sealed class DezibotHub(ISessionStore sessionStore) : Hub<IDezibotHubClient>
{
    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        // A new session is created when a client connects to the hub.
        await sessionStore.CreateActiveSessionAsync(Context.ConnectionId);
    }

    /// <inheritdoc />
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // The session is deactivated when the client disconnects from the hub.
        await sessionStore.DeactivateSessionAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}