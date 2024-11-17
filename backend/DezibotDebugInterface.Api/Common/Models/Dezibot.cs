namespace DezibotDebugInterface.Api.Common.Models;

public class Dezibot
{
    public string Ip { get; }
    
    public DateTime LastConnectionUtc { get; set; }
    
    public List<Debuggable> Debuggables { get; }
    
    public List<LogEntry> Logs { get; }
    
    public Dezibot(string ip, DateTime lastConnectionUtc, List<Debuggable>? debuggables = null, List<LogEntry>? logs = null)
    {
        Ip = ip;
        LastConnectionUtc = lastConnectionUtc;
        Debuggables = debuggables ?? [];
        Logs = logs ?? [];
    }
    
    public static void Update(Dezibot dezibotToUpdate, Dezibot newDezibot)
    {
        dezibotToUpdate.LastConnectionUtc = newDezibot.LastConnectionUtc;

        foreach (var newDebuggable in newDezibot.Debuggables)
        {
            var existingDebuggable = dezibotToUpdate.Debuggables.FirstOrDefault(debuggable => debuggable.Name == newDebuggable.Name);
            
            if (existingDebuggable is null)
            {
                dezibotToUpdate.Debuggables.Add(newDebuggable);
                continue;
            }

            foreach (var newProperty in newDebuggable.Properties)
            {
                var existingProperty = existingDebuggable.Properties.FirstOrDefault(property => property.Name == newProperty.Name);

                if (existingProperty is null)
                {
                    existingDebuggable.Properties.Add(newProperty);
                    continue;
                }

                var newTimeValues = newProperty.Values.Where(timeValue => !existingProperty.Values.Contains(timeValue));
                existingProperty.Values.AddRange(newTimeValues);
            }
        }
        
        var newLogEntries = newDezibot.Logs.Where(logEntry => !dezibotToUpdate.Logs.Contains(logEntry));
        dezibotToUpdate.Logs.AddRange(newLogEntries);
    }
    
    public record LogEntry(DateTime TimestampUtc, string LogLevel, string Message);

    public record Debuggable(string Name, List<Debuggable.Property> Properties)
    {
        public record Property(string Name, List<Property.TimeValue> Values)
        {
            public record TimeValue(DateTime TimestampUtc, string Value);
        }
    }
}