using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using DezibotDebugInterface.Api.DataAccess.Models;
using DezibotDebugInterface.Api.Tests.TestCommon;

using FluentAssertions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DezibotDebugInterface.Api.Tests.Endpoints.Sessions;

public class DeleteSessionEndpointsTests() : BaseDezibotTestFixture(nameof(DeleteSessionEndpointsTests))
{
    private const string DeleteAllNotUsedSessionsRoute = "/api/sessions";
    private const string DeleteSessionByIdRoute = "/api/session/{id:int}";
    private const string DeleteDezibotFromSessionRoute = "/api/session/{id:int}/dezibot/{ip}";
    
    [Fact]
    public async Task DeleteAllNotUsedSessionsAsync_WhenNoSessionsExists_ShouldDeleteAllNotUsedSessions()
    {
        // Act && Assert
        var response = await DeleteAsync<string>(DeleteAllNotUsedSessionsRoute, HttpStatusCode.OK);
        response.Should().Be("Deleted all sessions.");
    }
    
    [Fact]
    public async Task DeleteAllNotUsedSessionsAsync_WhenNotUsedSessionExists_ShouldDeleteAllNotUsedSessions()
    {
        // Arrange
        var existingSessions = SessionFactory.CreateSessions();
        existingSessions.ForEach(session => session.SessionClientConnections.Clear());
        
        await using(var arrangeDbContext = ResolveDbContext())
        {
            await arrangeDbContext.Sessions.AddRangeAsync(existingSessions);
            await arrangeDbContext.SaveChangesAsync();
        }
        
        // Act
        var response = await DeleteAsync<string>(DeleteAllNotUsedSessionsRoute, HttpStatusCode.OK);
        
        // Assert
        response.Should().Be("Deleted all sessions.");
        
        await using(var assertDbContext = ResolveDbContext())
        {
            var sessions = await assertDbContext.Sessions.ToListAsync();
            sessions.Should().BeEmpty();
        }
    }
    
    [Fact]
    public async Task DeleteAllNotUsedSessionsAsync_WhenUsedSessionExists_ShouldNotDeleteUsedSession()
    {
        // Arrange
        var existingSessions = SessionFactory.CreateSessions();
        
        await using(var arrangeDbContext = ResolveDbContext())
        {
            await arrangeDbContext.Sessions.AddRangeAsync(existingSessions);
            await arrangeDbContext.SaveChangesAsync();
        }
        
        // Act
        var response = await DeleteAsync<string>(DeleteAllNotUsedSessionsRoute, HttpStatusCode.OK);
        
        // Assert
        response.Should().Be("Deleted all sessions.");
        
        await using(var assertDbContext = ResolveDbContext())
        {
            var sessions = await assertDbContext.Sessions.ToListAsync();
            sessions.Should().HaveCount(existingSessions.Count);
        }
    }
    
    [Fact]
    public async Task DeleteSessionByIdAsync_WhenSessionNotExists_ShouldReturnNotFound()
    {
        // Act && Assert
        var response = await DeleteAsync<ProblemDetails>(DeleteSessionByIdRoute, HttpStatusCode.NotFound, id: 1);
        response.Should().NotBeNull();
        response!.Detail.Should().Be("Session with ID 1 not found.");
    }
    
    [Fact]
    public async Task DeleteSessionByIdAsync_WhenUsedSessionExists_ShouldReturnConflict()
    {
        // Arrange
        var existingSession = SessionFactory.CreateSession();
        
        await using(var arrangeDbContext = ResolveDbContext())
        {
            await arrangeDbContext.Sessions.AddAsync(existingSession);
            await arrangeDbContext.SaveChangesAsync();
        }
        
        // Act
        var response = await DeleteAsync<ProblemDetails>(DeleteSessionByIdRoute, HttpStatusCode.Conflict, existingSession.Id);
        
        // Assert
        response.Should().NotBeNull();
        response!.Detail.Should().Be($"Session with ID {existingSession.Id} has {existingSession.SessionClientConnections.Count} active clients using it and cannot be deleted.");
        
        await using(var assertDbContext = ResolveDbContext())
        {
            var session = await assertDbContext.Sessions.FindAsync(existingSession.Id);
            session.Should().NotBeNull();
        }
    }
    
    [Fact]
    public async Task DeleteSessionByIdAsync_WhenNotUsedSessionExists_ShouldDeleteSession()
    {
        // Arrange
        var existingSession = SessionFactory.CreateSession();
        existingSession.SessionClientConnections.Clear();
        
        await using(var arrangeDbContext = ResolveDbContext())
        {
            await arrangeDbContext.Sessions.AddAsync(existingSession);
            await arrangeDbContext.SaveChangesAsync();
        }
        
        // Act
        var response = await DeleteAsync<string>(DeleteSessionByIdRoute, HttpStatusCode.OK, existingSession.Id);
        
        // Assert
        response.Should().Be($"Deleted session with ID {existingSession.Id}.");
        
        await using(var assertDbContext = ResolveDbContext())
        {
            var session = await assertDbContext.Sessions.FindAsync(existingSession.Id);
            session.Should().BeNull();
        }
    }

    [Fact]
    public async Task DeleteDezibotFromSession_WhenSessionNotExists_ShouldReturnNotFound()
    {
        // Act && Assert
        var response = await DeleteAsync<ProblemDetails>(DeleteDezibotFromSessionRoute, HttpStatusCode.NotFound, id: 1, ip: "1.1.1.1");
        response.Should().NotBeNull();
        response!.Detail.Should().Be("The session with ID 1 does not exist.");
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeleteDezibotFromSession_WhenDezibotNotExistsInSession_ShouldReturnNotFound(bool existsInOtherSession)
    {
        // Arrange
        var existingSession = SessionFactory.CreateSession();
        Session? otherSession = null;

        await using(var arrangeDbContext = ResolveDbContext())
        {
            await arrangeDbContext.Sessions.AddAsync(existingSession);
            
            if (existsInOtherSession)
            {
                otherSession = SessionFactory.CreateSession();
                await arrangeDbContext.Sessions.AddAsync(otherSession);
            }
            
            await arrangeDbContext.SaveChangesAsync();
        }
        
        // Act
        var response = await DeleteAsync<ProblemDetails>(
            DeleteDezibotFromSessionRoute,
            HttpStatusCode.NotFound,
            existingSession.Id,
            ip: otherSession?.Dezibots[0].Ip ?? "2.2.2.2");
        
        // Assert
        response.Should().NotBeNull();
        response!.Detail.Should().Be($"The dezibot with IP {otherSession?.Dezibots[0].Ip ?? "2.2.2.2"} does not exist in session with ID {existingSession.Id.ToString()}.");
        
        if (existsInOtherSession)
        {
            await using var assertDbContext = ResolveDbContext();
            
            var session = await assertDbContext.Sessions
                .Include(session => session.Dezibots)
                .FirstOrDefaultAsync(session => session.Id == otherSession!.Id);
            
            session.Should().NotBeNull();
            session!.Dezibots.Should().HaveCount(1).And.Contain(dezibot => dezibot.Ip == otherSession!.Dezibots[0].Ip);
        }
    }
    
    [Fact]
    public async Task DeleteDezibotFromSession_WhenDezibotExistsInSession_ShouldDeleteDezibotFromSession()
    {
        // Arrange
        var existingSession = SessionFactory.CreateSession();
        var existingDezibot = existingSession.Dezibots[0];
        
        await using(var arrangeDbContext = ResolveDbContext())
        {
            await arrangeDbContext.Sessions.AddAsync(existingSession);
            await arrangeDbContext.SaveChangesAsync();
        }
        
        // Act
        var response = await DeleteAsync<string>(
            DeleteDezibotFromSessionRoute,
            HttpStatusCode.OK,
            existingSession.Id,
            existingDezibot.Ip);
        
        // Assert
        response.Should().Be($"Deleted dezibot with IP {existingDezibot.Ip} from session with ID {existingSession.Id}.");
        
        await using(var assertDbContext = ResolveDbContext())
        {
            var session = await assertDbContext.Sessions
                .Include(session => session.Dezibots)
                .FirstOrDefaultAsync(session => session.Id == existingSession.Id);
            
            session.Should().NotBeNull();
            session!.Dezibots.Should().BeEmpty();
        }
    }
    
    private async Task<TResponse?> DeleteAsync<TResponse>(
        string route,
        HttpStatusCode expectedStatusCode,
        int? id = null,
        string? ip = null)
    {
        var response = await HttpClient.DeleteAsync(BuildRoute(route, id, ip));
        response.StatusCode.Should().Be(expectedStatusCode);
        
        if (response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.Conflict)
        {
            var content = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(content) ? default : JsonSerializer.Deserialize<TResponse>(content, JsonSerializerOptions);
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(JsonSerializerOptions);
    }
}