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
    
    protected DezibotDbContext ResolveDbContext()
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

    public async Task InitializeAsync()
    {
        await _factory.CreateDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DeleteDatabaseAsync();
    }
}