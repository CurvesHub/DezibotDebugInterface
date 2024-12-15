namespace DezibotDebugInterface.Api.DataAccess.Models;

/// <summary>
/// Represents a time value.
/// </summary>
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
    private TimeValue() { }
}