using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.DataAccess.Models;

using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.Sessions;

/// <inheritdoc />
public class SessionStore(DezibotDbContext dbContext) : ISessionStore
{
    /// <inheritdoc />
    public async Task CreateActiveSessionAsync(string connectionId)
    {
        var session = new Session(connectionId);
        await dbContext.Sessions.AddAsync(session);
        await dbContext.SaveChangesAsync();
    }
    
    /// <inheritdoc />
    public async Task DeactivateSessionAsync(string connectionId)
    {
        var session = await dbContext.Sessions
            .Where(session => session.ClientConnectionId == connectionId)
            .FirstOrDefaultAsync();
        
        if (session is not null)
        {
            session.IsActive = false;
            await dbContext.SaveChangesAsync();
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Session>> GetActiveSessionAsync()
    {
        return await dbContext.Sessions
            .Where(session => session.IsActive)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Session>> GetAllSessionsAsync()
    {
        return await dbContext.Sessions.ToListAsync();
    }
}