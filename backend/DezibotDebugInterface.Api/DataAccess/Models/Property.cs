namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a property of a component.
/// </summary>
/// <param name="Name">The name of the property.</param>
/// <param name="Values">The values of the property.</param>
public record Property(
    string Name,
    List<DebugValue> Values)
{
    /// <summary>
    /// Maps a <see cref="Endpoints.Models.Property"/> to a <see cref="Property"/>.
    /// </summary>
    /// <param name="property">The property to map.</param>
    /// <returns>The mapped <see cref="Property"/>.</returns>
    public static Property FromProperty(Endpoints.Models.Property property)
    {
        return new Property(
            Name: property.Name,
            Values: property.Values.Select(DebugValue.FromDebugValue).ToList());
    }
}