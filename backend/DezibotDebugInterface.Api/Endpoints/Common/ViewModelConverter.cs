using DezibotDebugInterface.Api.DataAccess.Models;

namespace DezibotDebugInterface.Api.Endpoints.Common;

/// <summary>
/// Provides methods to convert to view models.
/// </summary>
public static class ViewModelConverter
{
    /// <summary>
    /// Converts a collection of <see cref="Dezibot"/>s to a collection of <see cref="DezibotViewModel"/>s.
    /// </summary>
    /// <param name="dezibots">The collection of <see cref="Dezibot"/>s to convert.</param>
    /// <returns>A collection of <see cref="DezibotViewModel"/>s.</returns>
    public static List<DezibotViewModel> ToDezibotViewModels(this IEnumerable<Dezibot> dezibots)
    {
        return dezibots.Select(dezibot => dezibot.ToDezibotViewModel()).ToList();
    }
    
    /// <summary>
    /// Converts a <see cref="Dezibot"/> to a <see cref="DezibotViewModel"/>.
    /// </summary>
    /// <param name="dezibot">The <see cref="Dezibot"/> to convert.</param>
    /// <returns>The converted <see cref="DezibotViewModel"/>.</returns>
    public static DezibotViewModel ToDezibotViewModel(this Dezibot dezibot)
    {
        return new DezibotViewModel(
            Ip: dezibot.Ip,
            LastConnectionUtc: dezibot.LastConnectionUtc.ToUnixTimeMilliseconds(),
            Logs: dezibot.Logs.Select(log => new LogEntryViewModel(
                TimestampUtc: log.TimestampUtc,
                Level: log.LogLevel.ToString(),
                ClassName: log.ClassName,
                Message: log.Message,
                Data: log.Data)).ToList(),
            Classes: dezibot.Classes.Select(@class => new ClassViewModel(
                Name: @class.Name,
                Properties: @class.Properties.Select(property => new PropertyViewModel(
                    Name: property.Name,
                    Values: property.Values.Select(value => new TimeValueViewModel(
                        TimestampUtc: value.TimestampUtc.ToUnixTimeMilliseconds(),
                        Value: value.Value)).ToList())).ToList())).ToList());
    }
}