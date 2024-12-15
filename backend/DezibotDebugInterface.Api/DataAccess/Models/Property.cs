namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a property of a Dezibot class.
/// </summary>
public class Property
{
    /// <summary>
    /// The unique identifier of the property.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// The name of the property.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// The values of the property.
    /// </summary>
    public List<TimeValue> Values { get; init; }
    
    /// <summary>
    /// Creates a new instance of the <see cref="Property"/> class.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="values">The values of the property.</param>
    public Property(string name, List<TimeValue> values)
    {
        Name = name;
        Values = values;
    }
            
    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    private Property() { }
}