using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

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
    public DateTimeOffset TimestampUtc { get; init; }

    /// <summary>
    /// The value.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Creates a new instance of the <see cref="TimeValue"/> class.
    /// </summary>
    /// <param name="timestampUtc">The timestamp of the value in UTC.</param>
    /// <param name="value">The value.</param>
    public TimeValue(DateTimeOffset timestampUtc, string value)
    {
        TimestampUtc = timestampUtc;
        Value = value;
    }
    
    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    [UsedImplicitly]
#pragma warning disable CS8618, CS9264
    private TimeValue() { }
#pragma warning restore CS8618, CS9264
}