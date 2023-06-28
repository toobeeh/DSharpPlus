using System.Threading.Tasks;
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
    public DiscordClient Client { get; init; }

    public DiscordChannel Channel { get; init; }
    public DiscordUser User { get; init; }
    public DiscordGuild? Guild { get; init; }
    public DiscordMember? Member { get; init; }

    public Task RespondAsync(DiscordMessageBuilder response);
    public Task DeferAsync();
    public Task FollowUpAsync(DiscordMessageBuilder response);
}
