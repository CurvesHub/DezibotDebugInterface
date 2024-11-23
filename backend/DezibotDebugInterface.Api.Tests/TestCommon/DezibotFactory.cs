using DezibotDebugInterface.Api.DataAccess;

namespace DezibotDebugInterface.Api.Tests.TestCommon;

/// <summary>
/// A factory for creating dezibots for testing purposes.
/// </summary>
public static class DezibotFactory
{
    private static readonly DateTime CurrentDateTime = DateTime.Parse("2024-01-01T00:00:00Z");

    /// <summary>
    /// Creates a dezibot.
    /// </summary>
    /// <param name="ip">The IP address of the dezibot.</param>
    /// <returns>A dezibot.</returns>
    public static Dezibot CreateDezibot(string? ip = null)
    {
        return new Dezibot(
            ip: ip ?? "1.1.1.1",
            lastConnectionUtc: CurrentDateTime,
            classes: [CreateClass(CurrentDateTime)],
            logs: [CreateLogEntry(CurrentDateTime)]);
    }
    
    /// <summary>
    /// Creates a list of dezibots.
    /// </summary>
    /// <param name="amount">The amount of dezibots to create.</param>
    /// <returns>A list of dezibots.</returns>
    public static List<Dezibot> CreateDezibots(int amount = 10)
    {
        return Enumerable
            .Range(1, amount)
            .Select(index =>
            {
                var newDateTime = CurrentDateTime.AddDays(index);

                return new Dezibot(
                    ip: $"{index}.{index}.{index}.{index}",
                    lastConnectionUtc: newDateTime,
                    classes: [CreateClass(newDateTime)],
                    logs: [CreateLogEntry(CurrentDateTime)]);
            }).ToList();
    }

    private static Dezibot.LogEntry CreateLogEntry(DateTime dateTime)
    {
        return new Dezibot.LogEntry(
            TimestampUtc: dateTime,
            ClassName: "Test class",
            Message: "Test message",
            Data: "Test data");
    }

    private static Dezibot.Class CreateClass(DateTime dateTime)
    {
        return new Dezibot.Class(
            name: "Test class",
            properties:
            [
                new Dezibot.Class.Property(
                    name: "Test property",
                    values:
                    [
                        new Dezibot.Class.Property.TimeValue(
                            TimestampUtc: dateTime,
                            Value: "Test value")
                    ])
            ]);
    }
}