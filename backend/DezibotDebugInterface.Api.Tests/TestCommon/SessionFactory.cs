using System.Globalization;

using DezibotDebugInterface.Api.DataAccess.Models;

#pragma warning disable S107 // Methods should not have too many parameters - This is a factory for creating test data.

namespace DezibotDebugInterface.Api.Tests.TestCommon;

/// <summary>
/// A factory for creating sessions for testing purposes.
/// </summary>
public static class SessionFactory
{
    private static readonly DateTimeOffset StartOf2024 = DateTimeOffset.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture);
    private static int _sessionId = 1;
    private static int _sessionClientConnectionId = 1;
    private static int _dezibotHubClientId = 1;

    /// <summary>
    /// Creates a session.
    /// </summary>
    /// <returns>A <see cref="Session"/>.</returns>
    public static Session CreateSession()
    {
        return CreateSessions(amount: 1)[0];
    }
   
    /// <summary>
    /// Creates a list of sessions.
    /// </summary>
    /// <param name="amount">The amount of sessions to create, will be passed to <see cref="DezibotFactory.CreateDezibots"/>.</param>
    /// <param name="createdUtc">The creation time of the sessions, if not specified, the time will be the start of 2024 advanced by one second for each entry.</param>
    /// <param name="clientConnectionId">The client connection ID of the sessions, if not specified, the client connection ID will be a new GUID.</param>
    /// <param name="dezibots">The dezibots of the sessions, if not specified, the dezibots will be created by <see cref="DezibotFactory.CreateDezibots"/>.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="Session"/>.</returns>
    public static List<Session> CreateSessions(
        int amount = 10,
        DateTimeOffset? createdUtc = null,
        string? clientConnectionId = null,
        Func<List<Dezibot>>? dezibots = null)
    {
        return Enumerable
            .Range(1, amount)
            .Select(index => new Session
            {
                Id = _sessionId++,
                CreatedUtc = createdUtc ?? StartOf2024.AddSeconds(index - 1),
                Dezibots = dezibots?.Invoke() ?? DezibotFactory.CreateDezibots(amount),
                SessionClientConnections = CreateSessionClientConnections(amount, clientConnectionId: clientConnectionId)
            })
            .ToList();
    }
    
    public static List<SessionClientConnection> CreateSessionClientConnections(
        int amount = 10,
        Session? session = null,
        string? clientConnectionId = null)
    {
        return Enumerable
            .Range(1, amount)
            .Select(index => new SessionClientConnection
            {
                Id = _sessionClientConnectionId++,
                Session = session,
                ReceiveUpdates = true,
                Client = new DezibotHubClient
                {
                    Id = _dezibotHubClientId++,
                    ConnectionId = clientConnectionId ?? Guid.NewGuid().ToString()
                }
            })
            .ToList();
    }
}