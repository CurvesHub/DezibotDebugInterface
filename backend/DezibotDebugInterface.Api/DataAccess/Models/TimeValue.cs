using System.Diagnostics.CodeAnalysis;

namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a time value.
/// </summary>
[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength", Justification = "The string fields wont be longer than 255 characters.")]
public class TimeValue
{
    /// <summary>
    /// The unique identifier of the time value.
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// The timestamp of the value in UTC.
    /// </summary>
    public required DateTimeOffset TimestampUtc { get; init; }

    /// <summary>
    /// The value.
    /// </summary>
    public required string Value { get; init; }
}