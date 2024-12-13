namespace DezibotDebugInterface.Api.DataAccess;

/// <summary>
/// Represents a Dezibot.
/// </summary>
public class Dezibot
{
    /// <summary>
    /// Creates a new instance of the <see cref="Dezibot"/> class.
    /// </summary>
    /// <param name="ip">The IP address of the Dezibot.</param>
    /// <param name="lastConnectionUtc">The last connection time of the Dezibot in UTC.</param>
    /// <param name="logs">The logs of the Dezibot.</param>
    /// <param name="classes">The classes of the Dezibot.</param>
    public Dezibot(
        string ip,
        DateTime lastConnectionUtc,
        List<LogEntry>? logs = null,
        List<Class>? classes = null)
    {
        Ip = ip;
        LastConnectionUtc = lastConnectionUtc;
        Logs = logs ?? [];
        Classes = classes ?? [];
    }

    /// <summary>
    /// Gets the IP address of the Dezibot, which uniquely identifies it.
    /// </summary>
    public string Ip { get; init; }
    
    /// <summary>
    /// Gets or sets the last connection time of the Dezibot in UTC.
    /// </summary>
    public DateTime LastConnectionUtc { get; set; }
    
    /// <summary>
    /// Gets the logs of the Dezibot.
    /// </summary>
    public List<LogEntry> Logs { get; init; }
    
    /// <summary>
    /// Get the classes of the Dezibot.
    /// </summary>
    public List<Class> Classes { get; init; }
    
    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    private Dezibot() { }
    
    /// <summary>
    /// Represents a log entry.
    /// </summary>
    /// <param name="TimestampUtc">The timestamp of the log message in UTC.</param>
    /// <param name="ClassName">The class name where the log message originated.</param>
    /// <param name="Message">The message of the log.</param>
    /// <param name="Data">Additional data of the log.</param>
    public record LogEntry(DateTime TimestampUtc, string ClassName, string Message, string? Data);

    /// <summary>
    /// Represents a Dezibot class.
    /// </summary>
    public record Class
    {
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
        /// The name of the class.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// The properties of the class.
        /// </summary>
        public List<Property> Properties { get; init; }
        
        /// <summary>
        /// Parameterless constructor for EF Core.
        /// </summary>
        public Class() { }
        
        /// <summary>
        /// Represents a property of a Dezibot class.
        /// </summary>
        public record Property
        {
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
            /// The name of the property.
            /// </summary>
            public string Name { get; init; }

            /// <summary>
            /// The values of the property.
            /// </summary>
            public List<TimeValue> Values { get; init; }
            
            /// <summary>
            /// Parameterless constructor for EF Core.
            /// </summary>
            public Property() { }
            
            /// <summary>
            /// Represents a time value.
            /// </summary>
            /// <param name="TimestampUtc">The timestamp of the value in UTC.</param>
            /// <param name="Value">The value.</param>
            public record TimeValue(DateTime TimestampUtc, string Value);
        }
    }
}