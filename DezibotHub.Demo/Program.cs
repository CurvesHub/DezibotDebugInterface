using Dumpify;

using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

var connection = new HubConnectionBuilder()
    .WithUrl("ws://localhost:5160/dezibot-hub")
    .WithAutomaticReconnect()
    .Build();


int counter = 0;

connection.On<Dezibot>("SendDezibotUpdateAsync", dezibot =>
{
    Console.WriteLine("Received dezibot update:");
    dezibot.Dump();
    Console.WriteLine($"Message count: {counter++}");
});


int maxRetries = 25;
while (connection.State != HubConnectionState.Connected && maxRetries-- > 0)
{
    try
    {
        await connection.StartAsync();
    }
    catch (Exception e)
    {
        Console.WriteLine(e.Message);
    }
}


app.Run();

public record Dezibot(
    string Ip,
    DateTime LastConnectionUtc,
    List<Dezibot.Debuggable> Debuggables,
    List<Dezibot.LogEntry> Logs)
{
    public record LogEntry(DateTime TimestampUtc, string LogLevel, string Message);

    public record Debuggable(string Name, List<Debuggable.Property> Properties)
    {
        public record Property(string Name, List<Property.TimeValue> Values)
        {
            public record TimeValue(DateTime TimestampUtc, string Value);
        }
    }
}