using DezibotHub.Demo;

using Microsoft.AspNetCore.SignalR.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

var connection = new HubConnectionBuilder()
    .WithUrl("ws://localhost:5160/api/dezibot-hub")
    .WithAutomaticReconnect()
    .Build();

int counter = 0;

connection.On<Dezibot>("DezibotUpdated", dezibot =>
{
    Console.WriteLine($"Received dezibot update {counter++}: {dezibot.Ip} - {dezibot.LastConnectionUtc} - Logs: {dezibot.Logs.Count} - Classes: {dezibot.Classes.Count}");
    Console.WriteLine();
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

app.MapGet("/joinSession", async () =>
{
    if (connection.State == HubConnectionState.Connected)
    {
        Console.WriteLine("Connected to hub, joining session with id: {4}");
        await connection.SendAsync("JoinSession", 4, true);
    }
    return Results.Ok();
});


app.Run();

namespace DezibotHub.Demo
{
    public record Dezibot(
        string Ip,
        long LastConnectionUtc,
        List<Dezibot.LogEntry> Logs,
        List<Dezibot.Class> Classes)
    {
        public record LogEntry(DateTime TimestampUtc, string Level, string ClassName, string Message, string? Data);

        public record Class(string Name, List<Class.Property> Properties)
        {
            public record Property(string Name, List<Property.TimeValue> Values)
            {
                public record TimeValue(long TimestampUtc, string Value);
            }
        }
    }
}
