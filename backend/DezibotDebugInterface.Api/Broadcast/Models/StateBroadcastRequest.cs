using DezibotDebugInterface.Api.Common.Models;

using JetBrains.Annotations;

namespace DezibotDebugInterface.Api.Broadcast.Models;

/// <summary>
/// The request to broadcast the state of a debuggable.
/// </summary>
/// <param name="Ip">The IP address of the dezibot.</param>
/// <param name="Debuggables">A list of json strings representing the state of the debuggables.</param>
[PublicAPI]
public record StateBroadcastRequest(string Ip, List<Dezibot.Debuggable> Debuggables);