using DeziBotDebugInterface.Api.Models;

namespace DeziBotDebugInterface.Api.Repositories;

/// <summary>
/// Provides data access for commands.
/// </summary>
public interface ICommandRepository
{
    /// <summary>
    /// Gets all commands available. Can be empty if no commands are available.
    /// </summary>
    /// <returns>An async enumerable of all available commands.</returns>
    IAsyncEnumerable<Command> GetCommandsAsync();
    
    /// <summary>
    /// Gets the commands for the specified Dezibot. Can be empty if no commands are available.
    /// </summary>
    /// <param name="serialNumber">The serial number of the Dezibot to get commands for.</param>
    /// <returns>An async enumerable of commands for the specified Dezibot.</returns>
    IAsyncEnumerable<Command> GetCommandsByIdAsync(string serialNumber);
}