using System.Net;
using System.Net.Http.Json;

using DezibotDebugInterface.Api.Endpoints.Sessions;
using DezibotDebugInterface.Api.Tests.TestCommon;

using FluentAssertions;

namespace DezibotDebugInterface.Api.Tests.Endpoints.Sessions;

public class CreateSessionEndpointTest() : BaseDezibotTestFixture(nameof(CreateSessionEndpointTest))
{
    private const string CreateSessionRoute = "/api/session";
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("TestSession")]
    public async Task CreateSessionAsync_WhenDifferentSessionNamesAreProvided_ShouldCreateSession(string? name)
    {
        // Arrange
        var request = new CreateSessionRequest(name!);
        
        // Act
        var response = await PostAsync(CreateSessionRoute, request, HttpStatusCode.Created);
        
        // Assert
        response.Should().NotBeNull();
        response!.Name.Should().Be(string.IsNullOrWhiteSpace(name) ? string.Empty : name);
    }
    
    private async Task<SessionIdentifier?> PostAsync(string route, CreateSessionRequest request, HttpStatusCode expectedStatusCode)
    {
        var response = await HttpClient.PostAsJsonAsync(route, request);
        response.StatusCode.Should().Be(expectedStatusCode);
        
        return await response.Content.ReadFromJsonAsync<SessionIdentifier>();
    } 
}