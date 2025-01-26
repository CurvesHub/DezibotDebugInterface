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
public sealed class DezibotHub(IServiceScopeFactory scopeFactory) : Hub<IDezibotHubClient>
{
    /// <summary>
    /// Allows a client to join a session and decide whether to receive updates or just the current session data.
    /// </summary>
    /// <param name="sessionId">The ID of the session to join.</param>
    /// <param name="continueSession">Whether to continue receiving updates.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task JoinSession(int? sessionId, bool continueSession)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (sessionId is null)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
            logger.Error("Client with connection ID {ConnectionId} tried to join a session without providing a session ID.", Context.ConnectionId);
            return;
        }

        var session = await dbContext.Sessions
            .Include(session => session.Dezibots)
            .Include(session => session.SessionClientConnections.Where(client => client.Client!.ConnectionId == Context.ConnectionId))
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (session is null)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
            logger.Error("Client with connection ID {ConnectionId} tried to join non-existent session {SessionId}.", Context.ConnectionId, sessionId);
            return;
        }
        
        if (session.SessionClientConnections.Count is 0)
        {
            session.SessionClientConnections.Add(new SessionClientConnection
            {
                Client = new DezibotHubClient { ConnectionId = Context.ConnectionId }
            });
        }

        session.SessionClientConnections[0].ReceiveUpdates = continueSession;
        await dbContext.SaveChangesAsync();
        
        foreach (var dezibotViewModel in session.Dezibots.ToDezibotViewModels())
        {
            await Clients.Caller.DezibotUpdated(dezibotViewModel);
        }
    }

    /// <inheritdoc />
    /// The client is removed when the connection is lost.
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await dbContext.Clients.Where(client => client.ConnectionId == Context.ConnectionId).ExecuteDeleteAsync();
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
            logger.Error(e, "Failed to remove client.");
            await transaction.RollbackAsync();
            throw;
        }

        await base.OnDisconnectedAsync(exception);
    }
}