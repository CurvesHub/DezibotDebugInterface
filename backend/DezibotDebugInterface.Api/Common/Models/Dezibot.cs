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
    
    public record LogEntry(DateTime TimestampUtc, string LogLevel, string Message);

    public record Debuggable(string Name, List<Debuggable.Property> Properties)
    {
        public record Property(string Name, List<Property.TimeValue> Values)
        {
            public record TimeValue(DateTime TimestampUtc, string Value);
        }
    }
}