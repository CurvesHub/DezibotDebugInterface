using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Endpoints.GetDezibot;

/// <summary>
/// Represents a view model for a Dezibot.
/// </summary>
/// <param name="Ip">The IP address of the Dezibot.</param>
/// <param name="LastConnectionUtc">The last connection time of the Dezibot in UTC as a Unix timestamp (milliseconds).</param>
/// <param name="Logs">The logs of the Dezibot.</param>
/// <param name="Classes">The classes of the Dezibot.</param>
[PublicAPI]
public record DezibotViewModel(string Ip, long LastConnectionUtc, List<LogEntryViewModel> Logs, List<ClassViewModel> Classes);

/// <summary>
/// Represents a view model for a log entry.
/// </summary>
/// <param name="TimestampUtc">The timestamp of the log entry in UTC as ISO 8601.</param>
/// <param name="Level">The log level of the log entry.</param>
/// <param name="ClassName">The class name where the log message originated.</param>
/// <param name="Message">The message of the log.</param>
/// <param name="Data">Additional data of the log.</param>
[PublicAPI]
public record LogEntryViewModel(DateTimeOffset TimestampUtc, string Level, string ClassName, string Message, string? Data);

/// <summary>
/// Represents a view model for a class.
/// </summary>
/// <param name="Name">The name of the class.</param>
/// <param name="Properties">The states of the class.</param>
[PublicAPI]
public record ClassViewModel(string Name, List<PropertyViewModel> Properties);

/// <summary>
/// Represents a view model for a property.
/// </summary>
/// <param name="Name">The name of the property.</param>
/// <param name="Values">The values of the property.</param>
[PublicAPI]
public record PropertyViewModel(string Name, List<TimeValueViewModel> Values);

/// <summary>
/// Represents a view model for a time value.
/// </summary>
/// <param name="TimestampUtc">The timestamp of the value in UTC as a Unix timestamp (milliseconds).</param>
/// <param name="Value">The value of the property at the given timestamp.</param>
[PublicAPI]
public record TimeValueViewModel(long TimestampUtc, string Value);