using System.Globalization;

using DezibotDebugInterface.Api.DataAccess.Models;

#pragma warning disable S107 // Methods should not have too many parameters - This is a factory for creating test data.

namespace DezibotDebugInterface.Api.Tests.TestCommon;

/// <summary>
/// A factory for creating dezibots for testing purposes.
/// </summary>
public static class DezibotFactory
{
    private static readonly DateTimeOffset StartOf2024 = DateTimeOffset.Parse("2024-01-01T00:00:00Z", CultureInfo.InvariantCulture);
    private static int _dezibotId = 1;
    private static int _logEntryId = 1;
    private static int _classId = 1;
    private static int _propertyId = 1;
    private static int _timeValueId = 1;

    /// <summary>
    /// Creates a dezibot.
    /// </summary>
    /// <returns>A <see cref="Dezibot"/>.</returns>
    public static Dezibot CreateDezibot(string? ip = null)
    {
        return CreateDezibots(amount: 1, ip)[0];
    }

    /// <summary>
    /// Creates a list of dezibots.
    /// </summary>
    /// <param name="amount">The amount of dezibots to create, will be passed to <see cref="CreateClasses"/> and <see cref="CreateLogEntries"/>.</param>
    /// <param name="ip">The IP of all dezibots, if not specified, the IP will be "{index}.{index}.{index}.{index}".</param>
    /// <param name="lastConnectionUtc">The last connection time of all dezibots, if not specified, the time will be the start of 2024 advanced by one second for each entry, will be passed to <see cref="CreateClasses"/> and <see cref="CreateLogEntries"/>.</param>
    /// <param name="classes">The classes of all dezibots, if not specified, the classes will be created by <see cref="CreateClasses"/>.</param>
    /// <param name="logs">The logs of all dezibots, if not specified, the logs will be created by <see cref="CreateLogEntries"/>.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="Dezibot"/>.</returns>
    public static List<Dezibot> CreateDezibots(
        int amount = 10,
        string? ip = null,
        DateTimeOffset? lastConnectionUtc = null,
        Func<List<Class>>? classes = null,
        Func<List<LogEntry>>? logs = null)
    {
        return Enumerable
            .Range(1, amount)
            .Select(index => new Dezibot(ip ?? $"{_dezibotId}.{_dezibotId}.{_dezibotId}.{_dezibotId}")
            {
                Id = _dezibotId++,
                LastConnectionUtc = lastConnectionUtc?.AddSeconds(index - 1) ?? StartOf2024.AddSeconds(index - 1),
                Classes = classes?.Invoke() ?? CreateClasses(amount: 1),
                Logs = logs?.Invoke() ?? CreateLogEntries(amount: 1)
            })
            .ToList();
    }

    /// <summary>
    /// Creates a list of log entries.
    /// </summary>
    /// <param name="amount">The amount of log entries to create.</param>
    /// <param name="timestampUtc">The timestamp of the log entries, if not specified, the time will be the start of 2024 advanced by one second for each entry.</param>
    /// <param name="logLevel">The log level of the log entries, if not specified, the log level will be <see cref="DezibotLogLevel.INFO"/>.</param>
    /// <param name="className">The class name of the log entries.</param>
    /// <param name="message">The message of the log entries.</param>
    /// <param name="data">The data of the log entries.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="LogEntry"/>.</returns>
    public static List<LogEntry> CreateLogEntries(
        int amount = 10,
        DateTimeOffset? timestampUtc = null,
        DezibotLogLevel? logLevel = null,
        string? className = null,
        string? message = null,
        string? data = null)
    {
        return Enumerable
            .Range(1, amount)
            .Select(index => new LogEntry(
                timestampUtc: timestampUtc?.AddSeconds(index - 1) ?? StartOf2024.AddSeconds(index - 1),
                logLevel: logLevel ?? DezibotLogLevel.INFO, 
                className: className ?? $"Class {index}",
                message: message ?? $"Message {index}",
                data: data ?? $"Data {index}")
            {
                Id = _logEntryId++
            })
            .ToList();
    }

    /// <summary>
    /// Creates a list of classes.
    /// </summary>
    /// <param name="amount">The amount of classes to create, will be passed to <see cref="CreateProperties"/>.</param>
    /// <param name="className">The name of the classes, if not specified, the name will be "Class {index}".</param>
    /// <param name="properties">The properties of the classes, if not specified, the properties will be created by <see cref="CreateProperties"/>.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="Class"/>.</returns>
    public static List<Class> CreateClasses(
        int amount = 10,
        string? className = null,
        Func<List<Property>>? properties = null)
    {
        return Enumerable
            .Range(1, amount)
            .Select(index => new Class(
                name: className ?? $"Class {index}",
                properties: properties?.Invoke() ?? CreateProperties(amount: 1))
            {
                Id = _classId++
            })
            .ToList();
    }

    /// <summary>
    /// Creates a list of properties.
    /// </summary>
    /// <param name="amount">The amount of properties to create, will be passed to <see cref="CreateTimeValues"/>.</param>
    /// <param name="propertyName">The name of the properties, if not specified, the name will be "Property {index}".</param>
    /// <param name="timeValues">The time values of the properties, if not specified, the time values will be created by <see cref="CreateTimeValues"/>.</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="Property"/>.</returns>
    public static List<Property> CreateProperties(
        int amount = 10,
        string? propertyName = null,
        Func<List<TimeValue>>? timeValues = null)
    {
        return Enumerable
            .Range(1, amount)
            .Select(index => new Property(
                name: propertyName ?? $"Property {index}",
                values: timeValues?.Invoke() ?? CreateTimeValues(amount: 1))
            {
                Id = _propertyId++
            })
            .ToList();
    }
    
    /// <summary>
    /// Creates a list of time values.
    /// </summary>
    /// <param name="amount">The amount of time values to create.</param>
    /// <param name="timestampUtc">The timestamp of all time values, if not specified, the time will be the start of 2024 advanced by one second for each entry.</param>
    /// <param name="value">The value of all time values, if not specified, the value will be "Value {index}".</param>
    /// <returns>A <see cref="List{T}"/> of <see cref="TimeValue"/>.</returns>
    public static List<TimeValue> CreateTimeValues(
        int amount = 10,
        DateTimeOffset? timestampUtc = null,
        string? value = null)
    {
        return Enumerable
            .Range(1, amount)
            .Select(index => new TimeValue(
                timestampUtc: timestampUtc?.AddSeconds(index - 1) ?? StartOf2024.AddSeconds(index - 1),
                value: value ?? $"Value {index}")
            {
                Id = _timeValueId++
            })
            .ToList();
    }
}