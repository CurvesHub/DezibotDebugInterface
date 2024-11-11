namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a component of a Dezibot.
/// </summary>
/// <param name="Name">The name of the component.</param>
/// <param name="Properties">The properties of the component.</param>
/// <param name="Logs">The logs of the component.</param>
public record Component(
    string Name,
    List<Property> Properties,
    List<Log> Logs)
{
    /// <summary>
    /// Maps a <see cref="Endpoints.Models.Component"/> to a <see cref="Component"/>.
    /// </summary>
    /// <param name="component">The component to map.</param>
    /// <returns>The mapped <see cref="Component"/>.</returns>
    public static Component FromComponent(Endpoints.Models.Component component)
    {
        return new Component(
            Name: component.Name,
            Properties: component.Properties.Select(Property.FromProperty).ToList(),
            Logs: component.Logs.Select(Log.FromLog).ToList());
    }
}