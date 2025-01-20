using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Endpoints.Sessions;

/// <summary>
/// Represents a session identifier.
/// </summary>
/// <param name="Id">The unique identifier of the session.</param>
/// <param name="Name">The name of the session.</param>
/// <param name="CreatedUtc">The date and time the session was created in UTC.</param>
[PublicAPI]
public record SessionIdentifier(
    int Id,
    string Name,
    DateTimeOffset CreatedUtc);