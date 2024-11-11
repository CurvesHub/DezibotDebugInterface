using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Endpoints.Models;

/// <summary>
/// The request for the <see cref="PutDezibotEndpoints"/> endpoint.
/// </summary>
/// <param name="Ip">The ip address of the Dezibot, which is a unique identifier.</param>
/// <param name="LastConnectionUtc">The last time the Dezibot connected to the server.</param>
/// <param name="Components">The components of the Dezibot.</param>
[PublicAPI]
public record PutDezibotRequest(
    string Ip,
    string LastConnectionUtc,
    IEnumerable<Component> Components);