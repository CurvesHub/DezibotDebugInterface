using System.Diagnostics.CodeAnalysis;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a Dezibot class.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "The string fields wont be longer than 255 characters.")]
public class Class
{
    /// <summary>
    /// The unique identifier of the class.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// The name of the class.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The properties of the class.
    /// </summary>
    public required List<Property> Properties { get; init; }
}