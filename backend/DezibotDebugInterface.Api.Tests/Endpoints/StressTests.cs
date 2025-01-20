using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

using DezibotDebugInterface.Api.Endpoints.Common;
using DezibotDebugInterface.Api.Endpoints.Sessions;
using DezibotDebugInterface.Api.Tests.TestCommon;

using FluentAssertions;

using Xunit.Abstractions;

namespace DezibotDebugInterface.Api.Tests.Endpoints;

// TODO: Fix test with new session handling
public class StressTests(ITestOutputHelper outputHelper) : BaseDezibotTestFixture(nameof(StressTests))
{
    [Theory]
    [InlineData(1, 1, 1,1, 1, 1, 300)]
    [InlineData(2, 1, 1,1, 1, 1, 300)]
    [InlineData(2, 2, 2,2, 2, 2, 500)]
    [InlineData(10, 10, 5,5, 10, 100, 2500)]
    public async Task GetAllSessionsAsync_WhenOneHundredSessionsExist_ShouldReturnSessionsInExpectedTime(
        int sessionCount,
        int dezibotCount,
        int classCount,
        int propertyCount,
        int timeValueCount,
        int logEntryCount,
        int expectedResponseTimeMilliseconds)
    {
        // Arrange
        var existingSessions = SessionFactory.CreateSessions(
            amount: sessionCount,
            dezibots: () => DezibotFactory.CreateDezibots(
                amount: dezibotCount,
                classes: () => DezibotFactory.CreateClasses(
                    amount: classCount,
                    properties: () => DezibotFactory.CreateProperties(
                        amount: propertyCount,
                        timeValues: () => DezibotFactory.CreateTimeValues(amount: timeValueCount))), 
                logs: () => DezibotFactory.CreateLogEntries(amount: logEntryCount)));
        
        await using var dbContext = ResolveDbContext();
        await dbContext.Sessions.AddRangeAsync(existingSessions);
        await dbContext.SaveChangesAsync();
        
        // Act
        var startTime = Stopwatch.GetTimestamp();
        var response = await HttpClient.GetAsync("/api/sessions");
        var responseTime = Stopwatch.GetElapsedTime(startTime);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseSessions = await response.Content.ReadFromJsonAsync<List<SessionViewModel>>(JsonSerializerOptions);
        responseSessions.Should().BeEquivalentTo(existingSessions.ToSessionViewModels());
        
        outputHelper.WriteLine($"Response Time: {responseTime}");
        responseTime.Should().BeLessThan(TimeSpan.FromMilliseconds(expectedResponseTimeMilliseconds));
    }
}