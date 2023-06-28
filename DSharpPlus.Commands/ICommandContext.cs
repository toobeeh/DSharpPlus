using DSharpPlus.Entities;

namespace DSharpPlus.Commands;

/// <summary>
/// A interface for command context
/// </summary>
/// <remarks>
/// Only one property should be null between interaction and message.
/// </remarks>
public interface ICommandContext
{
    public DiscordInteraction? Interaction { get; set; }
    public DiscordMessage? Message { get; set; }

    public CommandData CommandData { get; set; }
}
