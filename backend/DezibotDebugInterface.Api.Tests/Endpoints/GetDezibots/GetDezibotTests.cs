using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using DezibotDebugInterface.Api.Endpoints.GetDezibots;
using DezibotDebugInterface.Api.Tests.TestCommon;

using FluentAssertions;

namespace DezibotDebugInterface.Api.Tests.Endpoints.GetDezibots;

public class GetDezibotTests() : BaseDezibotTestFixture(nameof(GetDezibotTests))
{
    [Fact]
    public async Task GetAllDezibots_WhenDezibotsNotExist_ShouldReturnEmptyList()
    {
        // Act & Assert
        var dezibots = await GetAsync<List<DezibotViewModel>>(HttpStatusCode.OK);
        dezibots.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetDezibot_WhenDezibotNotExists_ShouldReturnNotFound()
    {
        // Act & Assert
        var dezibot = await GetAsync<DezibotViewModel>(HttpStatusCode.NotFound, "1.2.3.4");
        dezibot.Should().BeNull();
    }

    [Fact]
    public async Task GetAllDezibots_WhenDezibotsExist_ShouldReturnDezibots()
    {
        // Arrange
        var existingDezibots = DezibotFactory.CreateDezibots();

        await using var dbContext = ResolveDbContext();
        dbContext.Dezibots.AddRange(existingDezibots);
        await dbContext.SaveChangesAsync();

        // Act
        var dezibots = await GetAsync<List<DezibotViewModel>>(HttpStatusCode.OK);

        // Assert
        dezibots.Should().BeEquivalentTo(existingDezibots.Select(dezibot => dezibot.ToDezibotViewModel()));
    }

    [Fact]
    public async Task GetDezibot_WhenDezibotExists_ShouldReturnDezibot()
    {
        // Arrange
        var existingDezibot = DezibotFactory.CreateDezibot();
        
        await using var dbContext = ResolveDbContext();
        dbContext.Dezibots.Add(existingDezibot);
        await dbContext.SaveChangesAsync();
        
        // Act
        var dezibot = await GetAsync<DezibotViewModel>(HttpStatusCode.OK, existingDezibot.Ip);
        
        // Assert
        dezibot.Should().BeEquivalentTo(existingDezibot.ToDezibotViewModel());
    }
    
    private async Task<TResponse?> GetAsync<TResponse>(HttpStatusCode statusCode, string? ip = null)
    {
        var response = await HttpClient.GetAsync($"api/dezibots/{ip}");
        response.StatusCode.Should().Be(statusCode);
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            var content = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(content) ? default : JsonSerializer.Deserialize<TResponse>(content, JsonSerializerOptions);
        }
        
        var dezibots = await response.Content.ReadFromJsonAsync<TResponse>(JsonSerializerOptions);
        return dezibots;
    }
}