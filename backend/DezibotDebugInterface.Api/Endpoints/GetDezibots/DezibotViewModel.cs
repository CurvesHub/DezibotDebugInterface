using DezibotDebugInterface.Api.DataAccess;

namespace DezibotDebugInterface.Api.Endpoints.GetDezibots;

/// <summary>
/// Represents a view model for a Dezibot.
/// </summary>
/// <param name="Ip">The IP address of the Dezibot.</param>
/// <param name="LastConnectionUtc">The last connection time of the Dezibot in UTC as a Unix timestamp (milliseconds).</param>
/// <param name="Logs">The logs of the Dezibot.</param>
/// <param name="Classes">The classes of the Dezibot.</param>
public record DezibotViewModel(
    string Ip,
    long LastConnectionUtc,
    IEnumerable<Dezibot.LogEntry> Logs,
    IEnumerable<DezibotViewModel.Class> Classes)
{
    /// <summary>
    /// Represents a view model for a class.
    /// </summary>
    /// <param name="Name">The name of the class.</param>
    /// <param name="Properties">The states of the class.</param>
    public record Class(
        string Name,
        IEnumerable<Class.Property> Properties)
    {
        /// <summary>
        /// Represents a view model for a property.
        /// </summary>
        /// <param name="Name">The name of the property.</param>
        /// <param name="Values">The values of the property.</param>
        public record Property(
            string Name,
            IEnumerable<Property.TimeValue> Values)
        {
            /// <summary>
            /// Represents a view model for a time value.
            /// </summary>
            /// <param name="TimestampUtc">The timestamp of the value in UTC as a Unix timestamp (milliseconds).</param>
            /// <param name="Value">The value of the property at the given timestamp.</param>
            public record TimeValue(long TimestampUtc, string Value);
        }
    }
}