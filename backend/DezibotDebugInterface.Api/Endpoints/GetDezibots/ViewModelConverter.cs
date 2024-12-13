using DezibotDebugInterface.Api.DataAccess;

namespace DezibotDebugInterface.Api.Endpoints.GetDezibots;

/// <summary>
/// Provides methods to convert to view models.
/// </summary>
public static class ViewModelConverter
{
    /// <summary>
    /// Converts a <see cref="Dezibot"/> to a <see cref="DezibotViewModel"/>.
    /// </summary>
    /// <param name="dezibot">The <see cref="Dezibot"/> to convert.</param>
    /// <returns>The converted <see cref="DezibotViewModel"/>.</returns>
    public static DezibotViewModel ToDezibotViewModel(this Dezibot dezibot)
    {
        return new DezibotViewModel(
            Ip: dezibot.Ip,
            LastConnectionUtc: new DateTimeOffset(dezibot.LastConnectionUtc, offset: TimeSpan.Zero).ToUnixTimeMilliseconds(),
            Logs: dezibot.Logs.Select(log => new DezibotViewModel.LogEntry(
                TimestampUtc: new DateTimeOffset(log.TimestampUtc, offset: TimeSpan.Zero).ToUnixTimeMilliseconds(),
                ClassName: log.ClassName,
                Message: log.Message,
                Data: log.Data)),
            Classes: dezibot.Classes.Select(@class => new DezibotViewModel.Class(
                Name: @class.Name,
                Properties: @class.Properties.Select(property => new DezibotViewModel.Class.Property(
                    Name: property.Name,
                    Values: property.Values.Select(value => new DezibotViewModel.Class.Property.TimeValue(
                        TimestampUtc: new DateTimeOffset(value.TimestampUtc, offset: TimeSpan.Zero).ToUnixTimeMilliseconds(),
                        Value: value.Value)))))));
    }
}