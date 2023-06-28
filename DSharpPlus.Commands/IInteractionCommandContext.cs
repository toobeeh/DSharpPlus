using DSharpPlus.Entities;

namespace DSharpPlus.Commands;

public interface IInteractionCommandContext : ICommandContext
{
    public DiscordInteraction Interaction { get; set; }

}