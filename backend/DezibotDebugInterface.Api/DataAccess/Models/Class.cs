using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

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
    public string Name { get; init; }

    /// <summary>
    /// The properties of the class.
    /// </summary>
    public List<Property> Properties { get; init; }

    /// <summary>
    /// Creates a new instance of the <see cref="Class"/> class.
    /// </summary>
    /// <param name="name">The name of the class.</param>
    /// <param name="properties">The properties of the class.</param>
    public Class(string name, List<Property> properties)
    {
        Name = name;
        Properties = properties;
    }

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    [UsedImplicitly]
#pragma warning disable CS8618, CS9264
    private Class() { }
#pragma warning restore CS8618, CS9264
}