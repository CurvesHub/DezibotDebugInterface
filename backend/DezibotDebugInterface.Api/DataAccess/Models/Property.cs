using System.Diagnostics.CodeAnalysis;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a property of a Dezibot class.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "The string fields wont be longer than 255 characters.")]
public class Property
{
    /// <summary>
    /// The unique identifier of the property.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// The name of the property.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The values of the property.
    /// </summary>
    public required List<TimeValue> Values { get; init; }
}