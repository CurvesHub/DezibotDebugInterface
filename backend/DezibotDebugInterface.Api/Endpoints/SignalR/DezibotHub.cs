using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Endpoints.Common;

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using ILogger = Serilog.ILogger;

namespace DezibotDebugInterface.Api.Endpoints.SignalR;

/// <summary>
/// The SignalR hub for sending updates about dezibots.
/// </summary>
/// <param name="scopeFactory">The service scope factory.</param>
/// <param name="logger">The logger.</param>
public sealed class DezibotHub(IServiceScopeFactory scopeFactory, ILogger logger) : Hub<IDezibotHubClient>
{
    /// <summary>
    /// Allows a client to join a session and decide whether to receive updates or just the current session data.
    /// </summary>
    /// <param name="sessionId">The ID of the session to join.</param>
    /// <param name="continueSession">Whether to continue receiving updates.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task JoinSession(int sessionId, bool continueSession)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var session = await dbContext.Sessions
            .Include(session => session.Dezibots)
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session is null)
        {
            logger.Error("Client with connection ID {ConnectionId} tried to join non-existent session {SessionId}.", Context.ConnectionId, sessionId);
            return;
        }
        
        session.ClientConnections.Add(new HubClientConnection(Context.ConnectionId, continueSession));
        await dbContext.SaveChangesAsync();
        
        foreach (var dezibot in session.Dezibots)
        {
            await Clients.Caller.DezibotUpdated(dezibot.ToDezibotViewModel());
        }
    }

    /// <inheritdoc />
    /// The client is removed from the sessions when they disconnect.
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var sessions = await dbContext.Sessions
            .Where(session => session.ClientConnections.Any(connection => connection.ConnectionId == Context.ConnectionId))
            .Include(session => session.ClientConnections.Where(connection => connection.ConnectionId == Context.ConnectionId))
            .ToListAsync();

        foreach (var clientConnections in sessions.SelectMany(session => session.ClientConnections))
        {
            dbContext.RemoveRange(clientConnections);
        }
        
        await dbContext.SaveChangesAsync();
        await base.OnDisconnectedAsync(exception);
    }
}