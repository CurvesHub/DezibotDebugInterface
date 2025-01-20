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

    /// <summary>
    /// Creates a session.
    /// </summary>
    /// <returns>A <see cref="Session"/>.</returns>
    public static Session CreateSession(bool isActive = true)
    {
        return CreateSessions(amount: 1, isActive)[0];
    }
   
    /// <summary>
    /// Creates a list of sessions.
    /// </summary>
    /// <param name="amount">The amount of sessions to create, will be passed to <see cref="DezibotFactory.CreateDezibots"/>.</param>
    /// <param name="isActive">If the sessions are active, if not specified, the sessions will be active.</param>
    /// <param name="createdUtc">The creation time of the sessions, if not specified, the time will be the start of 2024 advanced by one second for each entry.</param>
    /// <param name="clientConnectionId">The client connection ID of the sessions, if not specified, the client connection ID will be a new GUID.</param>
    /// <param name="dezibots">The dezibots of the sessions, if not specified, the dezibots will be created by <see cref="DezibotFactory.CreateDezibots"/>.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="Session"/>.</returns>
    public static List<Session> CreateSessions( // TODO: Fix test with new session handling
        int amount = 10,
        bool? isActive = null,
        DateTimeOffset? createdUtc = null,
        string? clientConnectionId = null,
        Func<List<Dezibot>>? dezibots = null)
    {
        return Enumerable
            .Range(1, amount)
            .Select(index => new Session(clientConnectionId ?? Guid.NewGuid().ToString())
            {
                Id = _sessionId++,
                CreatedUtc = createdUtc ?? StartOf2024.AddSeconds(index - 1),
                Dezibots = dezibots?.Invoke() ?? DezibotFactory.CreateDezibots(amount: amount)
            })
            .ToList();
    }
}