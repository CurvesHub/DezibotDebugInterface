using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Endpoints.Sessions;

namespace DezibotDebugInterface.Api.Endpoints.Common;

/// <summary>
/// Provides methods to convert to view models.
/// </summary>
public static class ViewModelConverter
{
    /// <summary>
    /// Converts a collection of <see cref="Session"/>s to a collection of <see cref="SessionIdentifier"/>s.
    /// </summary>
    /// <param name="sessions">The collection of <see cref="Session"/>s to convert.</param>
    /// <returns>A collection of <see cref="SessionIdentifier"/>s.</returns>
    public static List<SessionIdentifier> ToSessionIdentifiers(this IEnumerable<Session> sessions)
    {
        return sessions.Select(session => session.ToSessionIdentifier()).ToList();
    }
    
    /// <summary>
    /// Converts a <see cref="Session"/> to a <see cref="SessionIdentifier"/>. 
    /// </summary>
    /// <param name="session">The <see cref="Session"/> to convert.</param>
    /// <returns>The converted <see cref="SessionIdentifier"/>.</returns>
    public static SessionIdentifier ToSessionIdentifier(this Session session)
    {
        return new SessionIdentifier(session.Id, session.Name ?? string.Empty, session.CreatedUtc);
    }

    /// <summary>
    /// Converts a collection of <see cref="Session"/>s to a collection of <see cref="SessionViewModel"/>s.
    /// </summary>
    /// <param name="sessions">The collection of <see cref="Session"/>s to convert.</param>
    /// <returns>A collection of <see cref="SessionViewModel"/>s.</returns>
    public static List<SessionViewModel> ToSessionViewModels(this IEnumerable<Session> sessions)
    {
        return sessions.Select(session => session.ToSessionViewModel()).ToList();
    }
    
    /// <summary>
    /// Converts a <see cref="Session"/> to a <see cref="SessionViewModel"/>.
    /// </summary>
    /// <param name="session">The <see cref="Session"/> to convert.</param>
    /// <returns>The converted <see cref="SessionViewModel"/>.</returns>
    public static SessionViewModel ToSessionViewModel(this Session session)
    {
        return new SessionViewModel(session.Id, session.Name ?? string.Empty, session.CreatedUtc, session.Dezibots.ToDezibotViewModels());
    }
    
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