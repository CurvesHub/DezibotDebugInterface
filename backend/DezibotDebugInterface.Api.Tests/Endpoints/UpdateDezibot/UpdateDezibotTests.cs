using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Endpoints.UpdateDezibot;
using DezibotDebugInterface.Api.Tests.TestCommon;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace DezibotDebugInterface.Api.Tests.Endpoints.UpdateDezibot;

public class UpdateDezibotTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DezibotWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly UpdateDezibotLogsRequest _logRequest;
    private readonly UpdateDezibotStatesRequest _stateRequest;
    
    public UpdateDezibotTests()
    {
        _factory = new DezibotWebApplicationFactory(nameof(UpdateDezibotTests));
        _client = _factory.CreateClient();
        _logRequest = new UpdateDezibotLogsRequest("1.1.1.1", /*DateTime.UtcNow,*/ "TestClass", "TestMessage", "TestData");
        _stateRequest = new UpdateDezibotStatesRequest("1.1.1.1", /*DateTime.UtcNow,*/ new Dictionary<string, Dictionary<string, string>> { { "TestClass", new Dictionary<string, string> { { "TestProperty", "TestValue" } } } });
    }
    
    [Fact]
    public async Task UpdateDezibotAsync_WhenRequestIsNull_ShouldReturnBadRequest()
    {
        // Arrange
        var expectedResponse = new ProblemDetails
        {
            Title = "Bad Request",
            Detail = "The request must contain either state or log data.",
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("api/dezibot/update", new {});
        
        // Assert
        var responseContent = await response.Content.ReadFromJsonAsync<ProblemDetails>(_jsonSerializerOptions);
        responseContent.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task UpdateDezibotAsync_WhenRequestIsWrongType_ShouldReturnBadRequest()
    {
        // Arrange
        var expectedResponse = new ProblemDetails
        {
            Title = "Bad Request",
            Detail = "The request must contain either state or log data.",
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        };
        
        // Act
        var response = await _client.PutAsJsonAsync("api/dezibot/update", new
        {
            logRequest = _logRequest,
            stateRequest = _stateRequest
        });
        
        // Assert
        var responseContent = await response.Content.ReadFromJsonAsync<ProblemDetails>(_jsonSerializerOptions);
        responseContent.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task UpdateLogs_WhenDezibotNotExists_ShouldCreateDezibotWithLogsAndReturnNoContent()
    {
        // Act
        var response = await _client.PutAsJsonAsync("api/dezibot/update", _logRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        
        var dezibot = await dbContext.Dezibots.FindAsync(_logRequest.Ip);
        dezibot.Should().NotBeNull();
        
        dezibot!.Logs.Should().ContainSingle(logEntry =>
            logEntry.TimestampUtc == dezibot.LastConnectionUtc/*_logRequest.TimestampUtc*/
            && logEntry.ClassName == _logRequest.ClassName
            && logEntry.Message == _logRequest.Message
            && logEntry.Data == _logRequest.Data);
    }
    
    [Fact]
    public async Task UpdateLogs_WhenSameRequestFiredTreeTimes_ShouldOnlySaveNewData()
    {
        // Act
        var response1 = await _client.PutAsJsonAsync("api/dezibot/update", _logRequest);
        var response2 = await _client.PutAsJsonAsync("api/dezibot/update", _logRequest);
        var response3 = await _client.PutAsJsonAsync("api/dezibot/update", _logRequest);
        
        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response2.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response3.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        
        var dezibot = await dbContext.Dezibots.FindAsync(_logRequest.Ip);
        dezibot.Should().NotBeNull();
        
        dezibot!.Logs.Should().ContainSingle(logEntry =>
            logEntry.TimestampUtc == dezibot.LastConnectionUtc/*_logRequest.TimestampUtc*/
            && logEntry.ClassName == _logRequest.ClassName
            && logEntry.Message == _logRequest.Message
            && logEntry.Data == _logRequest.Data);
    }
    
    [Fact]
    public async Task UpdateStates_WhenDezibotNotExists_ShouldCreateDezibotWithStatesAndReturnNoContent()
    {
        // Act
        var response = await _client.PutAsJsonAsync("api/dezibot/update", _stateRequest);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        
        var dezibot = await dbContext.Dezibots.FindAsync(_stateRequest.Ip);
        dezibot.Should().NotBeNull();
        StateShouldBeUpdated(dezibot!, _stateRequest);
    }
    
    [Obsolete("This test is obsolete because the timestamp is not used in the state update. Now the timestamp is set by the sever therefore each request is unique and will be saved with the same data.")]
    public async Task UpdateStates_WhenSameRequestFiredTreeTimes_ShouldOnlySaveNewData()
    {
        // Act
        var response1 = await _client.PutAsJsonAsync("api/dezibot/update", _stateRequest);
        var response2 = await _client.PutAsJsonAsync("api/dezibot/update", _stateRequest);
        var response3 = await _client.PutAsJsonAsync("api/dezibot/update", _stateRequest);
        
        // Assert
        response1.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response2.StatusCode.Should().Be(HttpStatusCode.NoContent);
        response3.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        
        var dezibot = await dbContext.Dezibots.FindAsync(_stateRequest.Ip);
        dezibot.Should().NotBeNull();
        StateShouldBeUpdated(dezibot!, _stateRequest);
    }
    
    [Fact]
    public async Task UpdateLogs_WhenDezibotExists_ShouldAddLogEntryAndReturnNoContent()
    {
        // Arrange
        List<Dezibot.LogEntry> existingLogs;
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();

            var existingDezibot = DezibotFactory.CreateDezibot(_logRequest.Ip);
            existingLogs = existingDezibot.Logs;
            
            dbContext.Add(existingDezibot);
            await dbContext.SaveChangesAsync();
        }
        
        // Act
        var response = await _client.PutAsJsonAsync("api/dezibot/update", _logRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var assertScope = _factory.Services.CreateAsyncScope();
        var assertDbContext = assertScope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        var dezibot = await assertDbContext.Dezibots.FindAsync(_logRequest.Ip);
        dezibot.Should().NotBeNull();

        dezibot!.Logs.Should().HaveCount(existingLogs.Count + 1).And.Contain(existingLogs);
        dezibot!.Logs.Should().Contain(logEntry =>
            logEntry.TimestampUtc == dezibot.LastConnectionUtc/*_logRequest.TimestampUtc*/
            && logEntry.ClassName == _logRequest.ClassName
            && logEntry.Message == _logRequest.Message
            && logEntry.Data == _logRequest.Data);
    }
    
    [Fact]
    public async Task UpdateStates_WhenDezibotExists_ShouldUpdateStatesAndReturnNoContent()
    {
        // Arrange
        List<Dezibot.Class> existingClasses;
        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();

            var existingDezibot = DezibotFactory.CreateDezibot(_stateRequest.Ip);
            existingClasses = existingDezibot.Classes;
            
            dbContext.Add(existingDezibot);
            await dbContext.SaveChangesAsync();
        }
        
        // Act
        var response = await _client.PutAsJsonAsync("api/dezibot/update", _stateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        await using var assertScope = _factory.Services.CreateAsyncScope();
        var assertDbContext = assertScope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        var dezibot = await assertDbContext.Dezibots.FindAsync(_stateRequest.Ip);
        dezibot.Should().NotBeNull();
        
        dezibot!.Classes.Should().HaveCount(existingClasses.Count + 1).And.ContainEquivalentOf(existingClasses[0]);
        StateShouldBeUpdated(dezibot!, _stateRequest);
    }
    
    [Fact]
    public async Task UpdateDezibotAsync_WhenRequestIsValid_ShouldSendDezibotUpdateToAllClients()
    {
        // Arrange
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
        var response = await _client.PutAsJsonAsync("api/dezibot/update", _logRequest);
        await Task.Delay(TimeSpan.FromSeconds(1));
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        
        var dezibot = dezibotMessages.Should().ContainSingle(dezibot => dezibot.Ip == _logRequest.Ip).Which;
        dezibot.Logs.Should().ContainSingle(log =>
            log.TimestampUtc == dezibot.LastConnectionUtc /*_logRequest.TimestampUtc*/
            && log.ClassName == _logRequest.ClassName
            && log.Message == _logRequest.Message
            && log.Data == _logRequest.Data);
        
        // Cleanup
        await connection.StopAsync();
        connection.State.Should().Be(HubConnectionState.Disconnected);
    }

    private static void StateShouldBeUpdated(Dezibot dezibot, UpdateDezibotStatesRequest stateRequest)
    {
        var newClass = new Dezibot.Class(
            name: stateRequest.Data.Keys.First(),
            properties: stateRequest.Data.Values.First().Select(property => new Dezibot.Class.Property(
                name: property.Key,
                values: [new Dezibot.Class.Property.TimeValue(dezibot.LastConnectionUtc,/*stateRequest.TimestampUtc,*/ property.Value)])).ToList());

        dezibot.Classes.Should()
            .ContainSingle(classState => classState.Name == newClass.Name)
            .Which.Should().BeEquivalentTo(newClass);
    }

    public Task InitializeAsync()
    {
        return _factory.CreateDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return _factory.DeleteDatabaseAsync();
    }
}