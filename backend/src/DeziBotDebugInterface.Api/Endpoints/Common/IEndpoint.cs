using JetBrains.Annotations;

namespace DeziBotDebugInterface.Api.Endpoints.Common;

/// <summary>
/// Defines an endpoint that can be mapped to the application.
/// </summary>
[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the provided <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder to map the endpoint to.</param>
    void MapEndpoints(IEndpointRouteBuilder endpoints);
}