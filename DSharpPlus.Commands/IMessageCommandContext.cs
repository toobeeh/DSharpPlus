using DSharpPlus.Entities;

namespace DSharpPlus.Commands;

/// <summary>
/// The interface that will be given if it is a message.
/// </summary>
public interface IMessageCommandContext : ICommandContext
{
    public DiscordMessage Message { get; init; }

}