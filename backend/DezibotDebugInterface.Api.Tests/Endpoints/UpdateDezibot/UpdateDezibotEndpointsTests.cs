using System.Net;
using System.Net.Http.Json;

using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Endpoints.Common;
using DezibotDebugInterface.Api.Endpoints.UpdateDezibot;
using DezibotDebugInterface.Api.Tests.TestCommon;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.Tests.Endpoints.UpdateDezibot;

public class UpdateDezibotEndpointsTests() : BaseDezibotTestFixture(nameof(UpdateDezibotEndpointsTests))
{
    private readonly UpdateDezibotLogsRequest _logRequest = new("1.1.1.1", DezibotLogLevel.INFO,"TestClass", "TestMessage", "TestData");
    private readonly UpdateDezibotStatesRequest _stateRequest = new("1.1.1.1", new Dictionary<string, Dictionary<string, string>>
    {
        { "TestClass", new Dictionary<string, string> { { "TestProperty", "TestValue" } } }
    });
    
    // TODO: Update when Session Tests are implemented
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task UpdateDezibotAsync_WhenRequestIsWrongType_ShouldReturnBadRequest(bool emptyRequest)
    {
        // Arrange
        var expectedResponse = new ProblemDetails
        {
            Title = "Bad Request",
            Detail = "The request must contain either state or log data.",
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        };
        
        var request = emptyRequest
            ? new object()
            : new { logRequest = _logRequest, stateRequest = _stateRequest };
        
        // Act
        var response = await UpdateDezibotAsync(request, HttpStatusCode.BadRequest);
        
        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Fact]
    public async Task UpdateLogs_WhenDezibotNotExists_ShouldCreateDezibotWithLogsAndReturnNoContent()
    {
        // Act
        await UpdateDezibotAsync(_logRequest, HttpStatusCode.NoContent);
        
        // Assert
        await using var dbContext = ResolveDbContext();
        var dezibot = await dbContext.Dezibots.Where(dezibot => dezibot.Ip == _logRequest.Ip).FirstOrDefaultAsync();
        
        dezibot.Should().NotBeNull();
        LogsContainSingle(dezibot!.Logs, dezibot.LastConnectionUtc);
    }
    
    [Fact]
    public async Task UpdateLogs_WhenSameRequestFiredTreeTimes_ShouldOnlySaveNewData()
    {
        // Act
        await UpdateDezibotAsync(_logRequest, HttpStatusCode.NoContent);
        await UpdateDezibotAsync(_logRequest, HttpStatusCode.NoContent);
        await UpdateDezibotAsync(_logRequest, HttpStatusCode.NoContent);
        
        // Assert
        await using var dbContext = ResolveDbContext();
        var dezibot = await dbContext.Dezibots.Where(dezibot => dezibot.Ip == _logRequest.Ip).FirstOrDefaultAsync();

        dezibot.Should().NotBeNull();
        LogsContainSingle(dezibot!.Logs, dezibot.LastConnectionUtc);
    }
    
    [Fact]
    public async Task UpdateStates_WhenDezibotNotExists_ShouldCreateDezibotWithStatesAndReturnNoContent()
    {
        // Act
        await UpdateDezibotAsync(_stateRequest, HttpStatusCode.NoContent);
        
        // Assert
        await using var dbContext = ResolveDbContext();
        var dezibot = await dbContext.Dezibots.Where(dezibot => dezibot.Ip == _logRequest.Ip).FirstOrDefaultAsync();
        
        dezibot.Should().NotBeNull();
        await ClassesContainSingle(dezibot!.Classes);
    }
    
    [Fact]
    public async Task UpdateLogs_WhenDezibotExists_ShouldAddLogEntryAndReturnNoContent()
    {
        // Arrange
        List<LogEntry> existingLogs;
        await using (var dbContext = ResolveDbContext())
        {
            var existingDezibot = DezibotFactory.CreateDezibot(_logRequest.Ip);
            
            dbContext.Add(existingDezibot);
            await dbContext.SaveChangesAsync();
            
            existingLogs = existingDezibot.Logs;
        }
        
        // Act
        await UpdateDezibotAsync(_logRequest, HttpStatusCode.NoContent);

        // Assert

        await using var assertDbContext = ResolveDbContext();
        var dezibot = await assertDbContext.Dezibots.Where(dezibot => dezibot.Ip == _logRequest.Ip).FirstOrDefaultAsync();

        dezibot.Should().NotBeNull();
        LogsContainSingle(dezibot!.Logs, dezibot.LastConnectionUtc);
        dezibot.Logs.Should().HaveCount(existingLogs.Count + 1).And.ContainEquivalentOf(existingLogs[0]);
    }
    
    [Fact]
    public async Task UpdateStates_WhenDezibotExists_ShouldUpdateStatesAndReturnNoContent()
    {
        // Arrange
        List<Class> existingClasses;
        await using (var dbContext = ResolveDbContext())
        {
            var existingDezibot = DezibotFactory.CreateDezibot(_stateRequest.Ip);
            
            dbContext.Add(existingDezibot);
            await dbContext.SaveChangesAsync();
            
            existingClasses = existingDezibot.Classes;
        }
        
        // Act
        await UpdateDezibotAsync(_stateRequest, HttpStatusCode.NoContent);

        // Assert
        await using var assertDbContext = ResolveDbContext();
        var dezibot = await assertDbContext.Dezibots.Where(dezibot => dezibot.Ip == _stateRequest.Ip).FirstOrDefaultAsync();
        
        dezibot.Should().NotBeNull();
        await ClassesContainSingle(dezibot!.Classes);
        dezibot.Classes.Should().HaveCount(existingClasses.Count + 1).And.ContainEquivalentOf(existingClasses[0]);
    }
    
    [Fact]
    public async Task UpdateDezibotAsync_WhenRequestIsValid_ShouldSendDezibotUpdateToAllClients()
    {
        // Arrange
        var connection = CreateHubConnection();
        
        List<DezibotViewModel> dezibotMessages = [];
        connection.On(
            methodName: "SendDezibotUpdateAsync",
            (DezibotViewModel dezibot) => dezibotMessages.Add(dezibot));

        await connection.StartAsync();
        connection.State.Should().Be(HubConnectionState.Connected);
        
        // Act
        await UpdateDezibotAsync(_logRequest, HttpStatusCode.NoContent);
        await Task.Delay(TimeSpan.FromSeconds(1));
        
        // Assert
        await using var dbContext = ResolveDbContext();
        var dezibot = await dbContext.Dezibots.Where(dezibot => dezibot.Ip == _logRequest.Ip).FirstOrDefaultAsync();
        var dezibotMessage = dezibotMessages.Should().ContainSingle(viewModel => viewModel.Ip == _logRequest.Ip).Which;
        
        dezibot.Should().NotBeNull();
        dezibot!.ToDezibotViewModel().Should().BeEquivalentTo(dezibotMessage);
        LogsContainSingle(
            dezibotMessage.Logs.Select(log => new LogEntry(
                log.TimestampUtc,
                Enum.Parse<DezibotLogLevel>(log.Level),
                log.ClassName,
                log.Message,
                log.Data)).ToList(),
            dezibot!.LastConnectionUtc);
        
        // Cleanup
        await connection.StopAsync();
        connection.State.Should().Be(HubConnectionState.Disconnected);
    }
    
    private async Task<ProblemDetails?> UpdateDezibotAsync<TRequest>(TRequest request, HttpStatusCode statusCode)
    {
        var response = await HttpClient.PutAsJsonAsync("api/dezibot/update", request);
        response.StatusCode.Should().Be(statusCode);

        if (statusCode is not HttpStatusCode.NoContent)
        {
            return await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonSerializerOptions);
        }

        var content = await response.Content.ReadAsStringAsync();
        content.Should().BeEmpty();
        return null;
    }
    
    private void LogsContainSingle(IReadOnlyCollection<LogEntry> logs, DateTimeOffset timestampUtc)
    {
        logs.Should().ContainSingle(log =>
            log.TimestampUtc == timestampUtc
            && log.ClassName == _logRequest.ClassName
            && log.Message == _logRequest.Message
            && log.Data == _logRequest.Data);
    }
    
    private async Task ClassesContainSingle(List<Class> classes)
    {
        await using var dbContext = ResolveDbContext();
        var expectedClass = await dbContext.Set<Class>()
            .Where(@class => @class.Name == _stateRequest.Data.Keys.Single())
            .Include(@class => @class.Properties)
            .ThenInclude(property => property.Values)
            .FirstOrDefaultAsync();
        
        expectedClass.Should().NotBeNull();

        classes.Should()
            .ContainSingle(classState => classState.Name == expectedClass!.Name)
            .Which.Should().BeEquivalentTo(expectedClass);
    }
}