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

// TODO: Fix test with new session handling
public class UpdateDezibotEndpointsTests() : BaseDezibotTestFixture(nameof(UpdateDezibotEndpointsTests))
{
    private readonly UpdateDezibotLogsRequest _logRequest = new("1.1.1.1", DezibotLogLevel.INFO,"TestClass", "TestMessage", "TestData");
    private readonly UpdateDezibotStatesRequest _stateRequest = new("1.1.1.1", new Dictionary<string, Dictionary<string, string>>
    {
        { "TestClass", new Dictionary<string, string> { { "TestProperty", "TestValue" } } }
    });
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task HandleDezibotUpdateAsync_WhenRequestIsWrongType_ShouldReturnBadRequest(bool emptyRequest)
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
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task HandleDezibotUpdateAsync_WhenIpAdressIsNullOrEmpty_ShouldReturnBadRequest(string? ip)
    {
        // Arrange
        var expectedResponse = new ProblemDetails
        {
            Title = "Bad Request",
            Detail = ip is null
                ? "The request must contain either state or log data."
                : "The IP address must not be null or empty.",
            Status = (int)HttpStatusCode.BadRequest,
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
        };
        
        var request = _logRequest with { Ip = ip! };
        
        // Act
        var response = await UpdateDezibotAsync(request, HttpStatusCode.BadRequest);
        
        // Assert
        response.Should().BeEquivalentTo(expectedResponse);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task HandleDezibotUpdateAsync_WhenNoActiveSessionExists_ShouldNotSaveData(bool isLogRequest)
    {
        // Arrange
        object request = isLogRequest ? _logRequest : _stateRequest;
        
        // Act
        await UpdateDezibotAsync(request, HttpStatusCode.NoContent);
        
        // Assert
        await using var dbContext = ResolveDbContext();
        var dezibot = await dbContext.Dezibots
            .FirstOrDefaultAsync(dezibot => dezibot.Ip == (isLogRequest ? _logRequest.Ip : _stateRequest.Ip));
        
        dezibot.Should().BeNull();
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task HandleDezibotUpdateAsync_WhenActiveSessionExistsAndDezibotNotExists_ShouldCreateNewDezibot(bool isLogRequest)
    {
        // Arrange
        var activeSession = SessionFactory.CreateSession();
        activeSession.Dezibots.Clear();
        
        await using (var dbContext = ResolveDbContext())
        {
            dbContext.Sessions.Add(activeSession);
            await dbContext.SaveChangesAsync();
        }
        
        object request = isLogRequest ? _logRequest : _stateRequest;

        // Act
        await UpdateDezibotAsync(request, HttpStatusCode.NoContent);
        
        // Assert
        await using var assertDbContext = ResolveDbContext();
        var dezibot = await assertDbContext.Dezibots
            .FirstOrDefaultAsync(dezibot => dezibot.Ip == (isLogRequest ? _logRequest.Ip : _stateRequest.Ip));
        
        dezibot.Should().NotBeNull();

        if (isLogRequest)
        {
            LogsContainSingle(dezibot!.Logs, dezibot.LastConnectionUtc);
        }
        else
        {
            await ClassesContainSingle(dezibot!.Classes);
        }
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task HandleDezibotUpdateAsync_WhenActiveSessionExistsAndDezibotExists_ShouldUpdateDezibot(bool isLogRequest)
    {
        // Arrange
        var activeSession = SessionFactory.CreateSession();
        var existingLogs = activeSession.Dezibots[0].Logs;
        var existingClasses = activeSession.Dezibots[0].Classes;
        
        await using (var dbContext = ResolveDbContext())
        {
            dbContext.Sessions.Add(activeSession);
            await dbContext.SaveChangesAsync();
        }
        
        object request = isLogRequest
            ? _logRequest with { Ip = activeSession.Dezibots[0].Ip }
            : _stateRequest with { Ip = activeSession.Dezibots[0].Ip };
        
        // Act
        await UpdateDezibotAsync(request, HttpStatusCode.NoContent);
        
        // Assert
        await using var assertDbContext = ResolveDbContext();
        var dezibot = await assertDbContext.Dezibots
            .FirstOrDefaultAsync(dezibot => dezibot.Ip == activeSession.Dezibots[0].Ip);
        
        dezibot.Should().NotBeNull();
        if (isLogRequest)
        {
            LogsContainSingle(dezibot!.Logs, dezibot.LastConnectionUtc);
            dezibot.Logs.Should().HaveCount(existingLogs.Count + 1).And.ContainEquivalentOf(existingLogs[0]);
        }
        else
        {
            await ClassesContainSingle(dezibot!.Classes);
            dezibot.Classes.Should().HaveCount(existingClasses.Count + 1).And.ContainEquivalentOf(existingClasses[0]);
        }
    }
   
        
    [Fact]
    public async Task UpdateLogs_WhenSameRequestFiredTreeTimes_ShouldOnlySaveNewData()
    {
        var activeSession = SessionFactory.CreateSession();
        activeSession.Dezibots.Clear();
        
        await using (var arrageDbContext = ResolveDbContext())
        {
            arrageDbContext.Sessions.Add(activeSession);
            await arrageDbContext.SaveChangesAsync();
        }
        
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
    public async Task HandleDezibotUpdateAsync_WhenActiveSessionExistsAndDezibotExists_ShouldSendDezibotUpdateToAllClients()
    {
        // Arrange
        await using var connection = CreateHubConnection();
        
        List<DezibotViewModel> dezibotMessages = [];
        connection.On(
            methodName: "SendDezibotUpdateAsync",
            (DezibotViewModel dezibot) => dezibotMessages.Add(dezibot));

        await connection.StartAsync();
        connection.State.Should().Be(HubConnectionState.Connected);
        
        await using(var arrangeDbContext = ResolveDbContext())
        {
            connection.ConnectionId.Should().NotBeNullOrEmpty();
            var activeSession = new Session(connection.ConnectionId!);
            activeSession.Dezibots.Add(new Dezibot(_logRequest.Ip, activeSession.Id));
            arrangeDbContext.Sessions.Add(activeSession);
            await arrangeDbContext.SaveChangesAsync();
        }
        
        // Act
        await UpdateDezibotAsync(_logRequest, HttpStatusCode.NoContent);
        await Task.Delay(TimeSpan.FromMilliseconds(200));
        
        // Assert
        await using var dbContext = ResolveDbContext();
        
        var session = await dbContext.Sessions.Include(session => session.Dezibots).FirstOrDefaultAsync();
        session.Should().NotBeNull();
        
        var dezibot = session!.Dezibots.Should().ContainSingle(dezibot => dezibot.Ip == _logRequest.Ip).Which;
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
        await Task.Delay(TimeSpan.FromMilliseconds(200));
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