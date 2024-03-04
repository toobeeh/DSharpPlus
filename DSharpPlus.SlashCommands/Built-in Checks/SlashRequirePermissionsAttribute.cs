using System;
using System.Threading.Tasks;

namespace DSharpPlus.SlashCommands.Attributes;

/// <summary>
/// Defines that usage of this slash command is restricted to members with specified permissions. This check also verifies that the bot has the same permissions.
/// </summary>
/// <remarks>
/// Defines that usage of this command is restricted to members with specified permissions. This check also verifies that the bot has the same permissions.
/// </remarks>
/// <param name="permissions">Permissions required to execute this command.</param>
/// <param name="ignoreDms">Sets this check's behaviour in DMs. True means the check will always pass in DMs, whereas false means that it will always fail.</param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class SlashRequirePermissionsAttribute(Permissions permissions, bool ignoreDms = true) : SlashCheckBaseAttribute
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

        Entities.DiscordMember usr = ctx.Member;
        if (usr == null)
        {
            return false;
        }

        Permissions pusr = ctx.Channel.PermissionsFor(usr);

        Entities.DiscordMember bot = await ctx.Guild.GetMemberAsync(ctx.Client.CurrentUser.Id);
        if (bot == null)
        {
            return false;
        }

        Permissions pbot = ctx.Channel.PermissionsFor(bot);

        bool usrok = ctx.Guild.OwnerId == usr.Id;
        bool botok = ctx.Guild.OwnerId == bot.Id;

        if (!usrok)
        {
            usrok = (pusr & Permissions.Administrator) != 0 || (pusr & this.Permissions) == this.Permissions;
        }

        if (!botok)
        {
            botok = (pbot & Permissions.Administrator) != 0 || (pbot & this.Permissions) == this.Permissions;
        }

        return usrok && botok;
    }
}
