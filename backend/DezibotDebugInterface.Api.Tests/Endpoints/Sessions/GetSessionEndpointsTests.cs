using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using DezibotDebugInterface.Api.Endpoints.Common;
using DezibotDebugInterface.Api.Endpoints.Sessions;
using DezibotDebugInterface.Api.Tests.TestCommon;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

namespace DezibotDebugInterface.Api.Tests.Endpoints.Sessions;

public class GetSessionEndpointsTests() : BaseDezibotTestFixture(nameof(GetSessionEndpointsTests))
{
    private const string GetAllSessionIdentifiersRoute = "/api/sessions/available";
    private const string GetAllSessionsRoute = "/api/sessions";
    private const string GetSessionByIdRoute = "/api/session/{id:int}";
    private const string GetDezibotByIpRoute = "/api/session/{id:int}/dezibot/{ip}";
    
    [Fact]
    public async Task GetAllSessionIdentifiersAsync_WhenSessionNotExists_ShouldReturnEmptyList()
    {
        // Act & Assert
        var sessionIdentifiers = await GetAsync<List<SessionIdentifier>>(GetAllSessionIdentifiersRoute, HttpStatusCode.OK);
        sessionIdentifiers.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetAllSessionIdentifiersAsync_WhenSessionExists_ShouldReturnSessionIdentifiers()
    {
        // Arrange
        var existingSessions = SessionFactory.CreateSessions();
        
        await using var dbContext = ResolveDbContext();
        await dbContext.Sessions.AddRangeAsync(existingSessions);
        await dbContext.SaveChangesAsync();
        
        // Act
        var sessionIdentifiers = await GetAsync<List<SessionIdentifier>>(GetAllSessionIdentifiersRoute, HttpStatusCode.OK);
        
        // Assert
        sessionIdentifiers.Should().BeEquivalentTo(existingSessions.ToSessionIdentifiers());
    }
    
    [Fact]
    public async Task GetAllSessionsAsync_WhenSessionsNotExists_ShouldReturnEmptyList()
    {
        // Act & Assert
        var sessions = await GetAsync<List<SessionViewModel>>(GetAllSessionsRoute, HttpStatusCode.OK);
        sessions.Should().BeEmpty();
    }
    
    [Fact]
    public async Task GetAllSessionsAsync_WhenSessionsExists_ShouldReturnSessions()
    {
        // Arrange
        var existingSessions = SessionFactory.CreateSessions();
        
        await using var dbContext = ResolveDbContext();
        await dbContext.Sessions.AddRangeAsync(existingSessions);
        await dbContext.SaveChangesAsync();
        
        // Act
        var sessions = await GetAsync<List<SessionViewModel>>(GetAllSessionsRoute, HttpStatusCode.OK);
        
        // Assert
        sessions.Should().BeEquivalentTo(existingSessions.ToSessionViewModels());
    }

    [Fact]
    public async Task GetSessionByIdAsync_WhenSessionNotExists_ShouldReturnNotFound()
    {
        // Act
        var problemDetails = await GetAsync<ProblemDetails>(GetSessionByIdRoute, HttpStatusCode.NotFound, id: 1);
        
        // Assert
        problemDetails.Should().NotBeNull();
        problemDetails!.Detail.Should().Be("Session with ID 1 not found.");
    }
    
    [Fact]
    public async Task GetSessionByIdAsync_WhenSessionExists_ShouldReturnSession()
    {
        // Arrange
        var existingSession = SessionFactory.CreateSession();
        
        await using var dbContext = ResolveDbContext();
        await dbContext.Sessions.AddAsync(existingSession);
        await dbContext.SaveChangesAsync();
        
        // Act
        var session = await GetAsync<SessionViewModel>(GetSessionByIdRoute, HttpStatusCode.OK, id: existingSession.Id);
        
        // Assert
        session.Should().BeEquivalentTo(existingSession.ToSessionViewModel());
    }

    [Fact]
    public async Task GetDezibotFromSession_WhenSessionNotExists_ShouldReturnNotFound()
    {
        // Act
        var problemDetails = await GetAsync<ProblemDetails>(
            GetDezibotByIpRoute,
            HttpStatusCode.NotFound,
            id: 1,
            ip: "1.1.1.1");
        
        // Assert
        problemDetails.Should().NotBeNull();
        problemDetails!.Detail.Should().Be("Session with ID 1 not found.");
    }

    [Fact]
    public async Task GetDezibotFromSession_WhenSessionExistsAndDezibotNotExists_ShouldReturnNotFound()
    {
        // Arrange
        var existingSession = SessionFactory.CreateSession();

        await using var dbContext = ResolveDbContext();
        await dbContext.Sessions.AddAsync(existingSession);
        await dbContext.SaveChangesAsync();

        // Act
        var problemDetails = await GetAsync<ProblemDetails>(
            GetDezibotByIpRoute,
            HttpStatusCode.NotFound,
            id: existingSession.Id,
            ip: "1.1.1.1");
        
        // Assert
        problemDetails.Should().NotBeNull();
        problemDetails!.Detail.Should().Be($"The dezibot with IP 1.1.1.1 was not found in session with ID {existingSession.Id}.");
    }

    [Fact]
    public async Task GetDezibotFromSession_WhenSessionExistsAndDezibotExists_ShouldReturnDezibot()
    {
        // Arrange
        var existingSession = SessionFactory.CreateSession();
        var existingDezibot = existingSession.Dezibots[0];

        await using var dbContext = ResolveDbContext();
        await dbContext.Sessions.AddAsync(existingSession);
        await dbContext.SaveChangesAsync();

        // Act
        var dezibot = await GetAsync<DezibotViewModel>(
            GetDezibotByIpRoute,
            HttpStatusCode.OK,
            id: existingSession.Id,
            ip: existingDezibot.Ip);
        
        // Assert
        dezibot.Should().BeEquivalentTo(existingDezibot.ToDezibotViewModel());
    }
    
    private async Task<TResponse?> GetAsync<TResponse>(
        string route,
        HttpStatusCode expectedStatusCode,
        int? id = null,
        string? ip = null)
    {
        var response = await HttpClient.GetAsync(BuildRoute(route, id, ip));
        response.StatusCode.Should().Be(expectedStatusCode);
        
        if (response.StatusCode is HttpStatusCode.NotFound)
        {
            var content = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(content) ? default : JsonSerializer.Deserialize<TResponse>(content, JsonSerializerOptions);
        }
        
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonSerializerOptions);
    }
}