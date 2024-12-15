using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;

using DezibotDebugInterface.Api.Endpoints.GetDezibots;
using DezibotDebugInterface.Api.Tests.TestCommon;

using FluentAssertions;

using Xunit.Abstractions;

namespace DezibotDebugInterface.Api.Tests.Endpoints;

public class StressTests(ITestOutputHelper outputHelper) : BaseDezibotTestFixture(nameof(StressTests))
{
    [Fact]
    public async Task GetAllDezibots_WhenADezibotHasOneThousandPropertyValuesAndLogs_ShouldReturnDezibotFast()
    {
        // Arrange
        // Create a dezibot with 10 classes, each with 10 properties, each with 10 time values, so 10 * 10 * 10 = 1000 time values and 1000 logs
        var dezibot = DezibotFactory.CreateDezibots(
            amount: 1,
            classes: DezibotFactory.CreateClasses(
                amount: 10,
                properties: DezibotFactory.CreateProperties(
                    amount: 10,
                    timeValues: DezibotFactory.CreateTimeValues(amount: 10))),
            logs: DezibotFactory.CreateLogEntries(amount: 1000))[0];

        await using var dbContext = ResolveDbContext();
        dbContext.Dezibots.Add(dezibot);
        await dbContext.SaveChangesAsync();
        
        // Act
        var startTime = Stopwatch.GetTimestamp();
        var response = await HttpClient.GetAsync($"api/dezibots/{dezibot.Ip}");
        var responseTime = Stopwatch.GetElapsedTime(startTime);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var responseDezibot = await response.Content.ReadFromJsonAsync<DezibotViewModel>(JsonSerializerOptions);
        responseDezibot.Should().BeEquivalentTo(dezibot.ToDezibotViewModel());
        
        outputHelper.WriteLine($"Response Time: {responseTime}");
        responseTime.Should().BeLessThan(TimeSpan.FromMilliseconds(500));
    }

    [Theory]
    [InlineData(10, 300)]
    [InlineData(50, 400)]
    [InlineData(100, 500)]
    public async Task StressTest_WhenAmountDezibotsExist_ShouldReturnDezibotsInExpectedTime(int amount, int expectedResponseTimeMilliseconds)
    {
        // Arrange
        var existingDezibots = DezibotFactory.CreateDezibots(amount);

        await using var dbContext = ResolveDbContext();
        dbContext.Dezibots.AddRange(existingDezibots);
        await dbContext.SaveChangesAsync();
        
        // Act
        var startTime = Stopwatch.GetTimestamp();
        var response = await HttpClient.GetAsync("api/dezibots");
        var responseTime = Stopwatch.GetElapsedTime(startTime);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var dezibots = await response.Content.ReadFromJsonAsync<List<DezibotViewModel>>(JsonSerializerOptions);
        dezibots.Should().BeEquivalentTo(existingDezibots.Select(dezibot => dezibot.ToDezibotViewModel()));
        
        outputHelper.WriteLine($"Response Time: {responseTime}");
        responseTime.Should().BeLessThan(TimeSpan.FromMilliseconds(expectedResponseTimeMilliseconds));
    }
}