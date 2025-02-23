using DezibotDebugInterface.Api.Endpoints.Common;

using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Endpoints.Sessions;

/// <summary>
/// Represents a session view model.
/// </summary>
/// <param name="Id">The unique identifier of the session.</param>
/// <param name="Name">The name of the session.</param>
/// <param name="CreatedUtc">The date and time the session was created in UTC.</param>
/// <param name="Dezibots">The dezibots associated with this session.</param>
[PublicAPI]
public record SessionViewModel(
    int Id,
    string Name,
    DateTimeOffset CreatedUtc,
    List<DezibotViewModel> Dezibots);