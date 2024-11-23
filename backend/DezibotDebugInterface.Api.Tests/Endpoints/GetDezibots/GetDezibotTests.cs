using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using DezibotDebugInterface.Api.DataAccess;
using DezibotDebugInterface.Api.Tests.TestCommon;

using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;

namespace DezibotDebugInterface.Api.Tests.Endpoints.GetDezibots;

public class GetDezibotTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DezibotWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    
    public GetDezibotTests()
    {
        _factory = new DezibotWebApplicationFactory(nameof(GetDezibotTests));
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllDezibots_WhenDezibotsNotExist_ShouldReturnEmptyList()
    {
        // Act
        var response = await _client.GetAsync("api/dezibots");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var dezibots = await response.Content.ReadFromJsonAsync<List<Dezibot>>(_jsonSerializerOptions);
        dezibots.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllDezibots_WhenDezibotsExist_ShouldReturnDezibots()
    {
        // Arrange
        var existingDezibots = DezibotFactory.CreateDezibots();
        
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        
        dbContext.Dezibots.AddRange(existingDezibots);
        await dbContext.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync("api/dezibots");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var dezibots = await response.Content.ReadFromJsonAsync<List<Dezibot>>(_jsonSerializerOptions);
        dezibots.Should().BeEquivalentTo(existingDezibots);
    }

    [Fact]
    public async Task GetDezibot_WhenDezibotNotExists_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("api/dezibots/1.1.1.1");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDezibot_WhenDezibotExists_SouldReturnDezibot()
    {
        // Arrange
        var existingDezibot = DezibotFactory.CreateDezibot();
        
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DezibotDbContext>();
        
        dbContext.Dezibots.Add(existingDezibot);
        await dbContext.SaveChangesAsync();
        
        // Act
        var response = await _client.GetAsync($"api/dezibots/{existingDezibot.Ip}");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var dezibot = await response.Content.ReadFromJsonAsync<Dezibot>(_jsonSerializerOptions);
        dezibot.Should().BeEquivalentTo(existingDezibot);
    }

    public async Task InitializeAsync()
    {
        await _factory.CreateDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DeleteDatabaseAsync();
    }
}