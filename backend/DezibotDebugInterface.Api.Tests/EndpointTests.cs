using System.ComponentModel;
using System.Net.Http.Json;
using System.Text.Json;

using DezibotDebugInterface.Api.Broadcast.Models;
using DezibotDebugInterface.Api.Common.DataAccess;
using DezibotDebugInterface.Api.Common.Models;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DezibotDebugInterface.Api.Tests;

public class EndpointTests
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    public EndpointTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [Description(
        """
        Scenario:
            - A LogBroadcastRequest is received.
            - The bot either exists or not.
            
        Expected:
            - The bot is created/updated, saved and can be retrieved.
        """)]
    public async Task LogBroadcastRequestReceived_BotDoesExistOrNot_BotIsCreatedSavedAndCanBeRetrieved(bool botExists)
    {
        // Arrange
        var logBroadcastRequest = new LogBroadcastRequest(
            Ip: "1.1.1.1",
            TimestampUtc: DateTime.UtcNow,
            LogLevel: "INFO",
            Message: "Test message");
        
        await using var scope = _factory.Services.CreateAsyncScope();
        var dezibotRepository = scope.ServiceProvider.GetRequiredService<IDezibotRepository>();

        if (botExists)
        {
            await dezibotRepository.UpdateAsync(logBroadcastRequest.Ip, logs:
            [
                new Dezibot.LogEntry(logBroadcastRequest.TimestampUtc, logBroadcastRequest.LogLevel, logBroadcastRequest.Message)
            ]);
        }
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/broadcast/logs", logBroadcastRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var dezibot = await dezibotRepository.GetByIpAsync(logBroadcastRequest.Ip);
        dezibot.Should().NotBeNull();
        dezibot!.Logs.Should().ContainSingle(log =>
            log.TimestampUtc == logBroadcastRequest.TimestampUtc &&
            log.LogLevel == logBroadcastRequest.LogLevel &&
            log.Message == logBroadcastRequest.Message);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [Description(
        """
        Scenario:
            - A StateBroadcastRequest is received.
            - The bot either exists or not.
            
        Expected:
            - The bot is created/updated, saved and can be retrieved.
        """)]
    public async Task StateBroadcastRequestReceived_BotDoesExistOrNot_BotIsCreatedSavedAndCanBeRetrieved(bool botExists)
    {
        // Arrange
        var stateBroadcastRequest = new StateBroadcastRequest(
            Ip: "1.1.1.1",
            Debuggables: [CreateDebuggable(DateTime.UtcNow)]);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dezibotRepository = scope.ServiceProvider.GetRequiredService<IDezibotRepository>();
        
        if (botExists)
        {
            await dezibotRepository.UpdateAsync(stateBroadcastRequest.Ip, stateBroadcastRequest.Debuggables);
        }

        // Act
        var response = await _client.PutAsJsonAsync("/api/broadcast/states", stateBroadcastRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        
        var dezibot = await dezibotRepository.GetByIpAsync(stateBroadcastRequest.Ip);
        dezibot.Should().NotBeNull();
        dezibot!.Debuggables.Should().BeEquivalentTo(stateBroadcastRequest.Debuggables);
    }
    
    [Fact]
    public async Task GetDezibots_WhenOneOrAllDezibotsAreRequested_TheDezibotsAreReturned()
    {
        // Arrange
        var dezibots = CreateDezibots();
        
        await using var scope = _factory.Services.CreateAsyncScope();
        var dezibotRepository = scope.ServiceProvider.GetRequiredService<IDezibotRepository>();
        
        foreach (var dezibot in dezibots)
        {
            await dezibotRepository.UpdateAsync(dezibot.Ip, dezibot.Debuggables, dezibot.Logs);
        }
        
        // Act
        var response = await _client.GetAsync("/api/dezibots");
        var repositoryDezibots = await dezibotRepository.GetAllDezibotsAsync().ToListAsync();
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<List<Dezibot>>(content, _jsonSerializerOptions);

        result.Should().BeEquivalentTo(dezibots);
        repositoryDezibots.Should().BeEquivalentTo(dezibots);
    }
    
    [Fact]
    public async Task BroadcastRequestReceived_WhenBotIsUpdated_TheUpdatedBotIsBroadcastedToAllDezibotHubClients()
    {
        // Arrange
        var logBroadcastRequest = new LogBroadcastRequest(
            Ip: "1.1.1.1",
            TimestampUtc: DateTime.UtcNow,
            LogLevel: "INFO",
            Message: "Test message");

        var connection = new HubConnectionBuilder()
            .WithUrl("ws://localhost:80/dezibot-hub", options =>
            {
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
            })
            .Build();
        
        List<Dezibot> dezibotMessages = [];
        connection.On("SendDezibotUpdateAsync", (Dezibot dezibot) => dezibotMessages.Add(dezibot));

        await connection.StartAsync();
        connection.State.Should().Be(HubConnectionState.Connected);
        
        // Act
        var response = await _client.PutAsJsonAsync("/api/broadcast/logs", logBroadcastRequest);
        
        // Assert
        response.EnsureSuccessStatusCode();
        await connection.StopAsync();
        connection.State.Should().Be(HubConnectionState.Disconnected);

        dezibotMessages.Should().ContainSingle(dezibot => dezibot.Ip == logBroadcastRequest.Ip).Which
            .Logs.Should().ContainSingle(log =>
                log.TimestampUtc == logBroadcastRequest.TimestampUtc &&
                log.LogLevel == logBroadcastRequest.LogLevel &&
                log.Message == logBroadcastRequest.Message);
    }

    private static List<Dezibot> CreateDezibots(int amount = 10)
    {
        const string timestamp = "2022-01-01T00:00:00Z";
        var dateTime = DateTime.Parse(timestamp);

        return Enumerable
            .Range(1, amount)
            .Select(index =>
            {
                var newDateTime = dateTime.AddDays(index);
                
                return new Dezibot(
                    ip: $"{index}.{index}.{index}.{index}",
                    lastConnectionUtc: newDateTime,
                    debuggables:
                    [
                        CreateDebuggable(newDateTime)
                    ],
                    logs:
                    [
                        new Dezibot.LogEntry(dateTime, "INFO", $"Test message {index}")
                    ]);
            }).ToList();
    }
    
    private static Dezibot.Debuggable CreateDebuggable(DateTime timestampUtc)
    {
        return new Dezibot.Debuggable(
            Name: "Test debuggable",
            Properties:
            [
                new Dezibot.Debuggable.Property(
                    Name: "Test property", 
                    Values:
                    [
                        new Dezibot.Debuggable.Property.TimeValue(
                            TimestampUtc: timestampUtc,
                            Value: "Test value")
                    ])
            ]);
    }
}