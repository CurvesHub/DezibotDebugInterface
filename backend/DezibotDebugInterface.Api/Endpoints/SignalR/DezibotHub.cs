using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.Endpoints.SignalR;

/// <summary>
/// The SignalR hub for sending updates about dezibots.
/// </summary>
/// <param name="scopeFactory">The service scope factory.</param>
public sealed class DezibotHub(IServiceScopeFactory scopeFactory) : Hub<IDezibotHubClient>
{
    /// <inheritdoc />
    /// A new session is created when a new client connects to the hub.
    public override async Task OnConnectedAsync()
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        
        string connectionId = Context.ConnectionId;
        
        // If the same client reconnects, the session is reactivated.
        var session = await dbContext.Sessions
            .Where(session => session.ClientConnectionId == connectionId)
            .FirstOrDefaultAsync();

        if (session is null)
        {
            session = new Session(connectionId);
            await dbContext.Sessions.AddAsync(session);
        }
        
        session.IsActive = true;
        await dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    /// The client's session is deactivated when it disconnects from the hub.
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        
        string connectionId = Context.ConnectionId;
        
        var session = await dbContext.Sessions
            .Where(session => session.ClientConnectionId == connectionId)
            .FirstOrDefaultAsync();

        if (session is not null)
        {
            session.IsActive = false;
            await dbContext.SaveChangesAsync();
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}