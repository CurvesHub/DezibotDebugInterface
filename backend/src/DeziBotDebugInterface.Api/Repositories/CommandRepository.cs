using DeziBotDebugInterface.Api.Models;

namespace DeziBotDebugInterface.Api.Repositories;

/// <inheritdoc />
public class CommandRepository : ICommandRepository // TODO: Implement missing methods
{
    /// <inheritdoc />
    public IAsyncEnumerable<Command> GetCommandsAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Command> GetCommandsByIdAsync(string serialNumber)
    {
        throw new NotImplementedException();
    }
}