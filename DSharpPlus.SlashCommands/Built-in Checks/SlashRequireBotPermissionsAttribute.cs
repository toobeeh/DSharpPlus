using System;
using System.Threading.Tasks;

namespace DSharpPlus.SlashCommands.Attributes;

/// <summary>
/// Defines that usage of this slash command is only possible when the bot is granted a specific permission.
/// </summary>
/// <remarks>
/// Defines that usage of this slash command is only possible when the bot is granted a specific permission.
/// </remarks>
/// <param name="permissions">Permissions required to execute this command.</param>
/// <param name="ignoreDms">Sets this check's behaviour in DMs. True means the check will always pass in DMs, whereas false means that it will always fail.</param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SlashRequireBotPermissionsAttribute(Permissions permissions, bool ignoreDms = true) : SlashCheckBaseAttribute
{
    /// <summary>
    /// Gets the permissions required by this attribute.
    /// </summary>
    public Permissions Permissions { get; } = permissions;

    /// <summary>
    /// Gets or sets this check's behaviour in DMs. True means the check will always pass in DMs, whereas false means that it will always fail.
    /// </summary>
    public bool IgnoreDms { get; } = ignoreDms;

    /// <summary>
    /// Runs checks.
    /// </summary>
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        if (ctx.Guild == null)
        {
            return this.IgnoreDms;
        }

        Entities.DiscordMember bot = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
        if (bot == null)
        {
            return false;
        }

        if (bot.Id == ctx.Guild.OwnerId)
        {
            return true;
        }

        Permissions pbot = ctx.Channel.PermissionsFor(bot);

        return (pbot & Permissions.Administrator) != 0 ? true : (pbot & this.Permissions) == this.Permissions;
    }
}
