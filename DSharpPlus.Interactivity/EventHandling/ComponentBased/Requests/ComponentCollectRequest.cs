using System;
using System.Collections.Concurrent;
using System.Threading;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
namespace DSharpPlus.Interactivity.EventHandling;

/// <summary>
/// Represents a component event that is being waited for.
/// </summary>
internal sealed class ComponentCollectRequest(DiscordMessage message, Func<ComponentInteractionCreateEventArgs, bool> predicate, CancellationToken cancellation) : ComponentMatchRequest(message, predicate, cancellation)
{
    public ConcurrentBag<ComponentInteractionCreateEventArgs> Collected { get; private set; }
}
