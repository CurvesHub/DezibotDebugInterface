using System.Text.Json;

using DezibotDebugInterface.Api.DataAccess;

using Microsoft.AspNetCore.SignalR.Client;

namespace DezibotDebugInterface.Api.Tests.TestCommon;

public class BaseDezibotTestFixture : IAsyncLifetime
{
    private readonly DezibotApiFactory _factory;
    
    /// <summary>
    /// Gets a pre-configured shared <see cref="HttpClient"/> for all tests.
    /// </summary>
    protected HttpClient HttpClient { get; private set; }
    
    /// <summary>
    /// The JSON serializer options to use for deserialization.
    /// </summary>
    protected static readonly JsonSerializerOptions JsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };
    
    protected BaseDezibotTestFixture(string testName)
    {
        _factory = new DezibotApiFactory(testName);
        HttpClient = _factory.CreateClient();
    }
    
    protected ApplicationDbContext ResolveDbContext()
    {
        return _factory.ResolveDbContext();
    }
    
    protected HubConnection CreateHubConnection()
    {
        return new HubConnectionBuilder()
            .WithUrl("ws://localhost:8080/api/dezibot-hub", options =>
            {
                options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
            })
            .Build();
    }
   
    /// <summary>
    /// Builds a route with the provided parameters.
    /// </summary>
    /// <param name="route">The route to use.</param>
    /// <param name="id">The ID to insert into the route.</param>
    /// <param name="ip">The IP address to insert into the route.</param>
    /// <example>"/api/session/{id:int}/dezibot/{ip}" with id = 1 and ip = "1.1.1.1" will return "/api/session/1/dezibot/1.1.1.1</example>
    /// <returns>The route with the provided parameters inserted.</returns>
    protected static string BuildRoute(string route, int? id = null, string? ip = null)
    {
        if (id is null)
        {
            return route;
        }
        
        if(ip is null)
        {
            return route.Replace("{id:int}", id.ToString());
        }
        
        return route
            .Replace("{id:int}", id.ToString())
            .Replace("{ip}", ip);
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