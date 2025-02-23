using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Endpoints.Sessions;

/// <summary>
/// Represents a request to create a new session.
/// </summary>
/// <param name="Name">The name of the session.</param>
[PublicAPI]
public record CreateSessionRequest(string Name);