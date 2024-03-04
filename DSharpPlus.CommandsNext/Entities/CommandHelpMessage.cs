using DSharpPlus.Entities;

namespace DSharpPlus.CommandsNext.Entities;

/// <summary>
/// Represents a formatted help message.
/// </summary>
/// <remarks>
/// Creates a new instance of a help message.
/// </remarks>
/// <param name="content">Contents of the message.</param>
/// <param name="embed">Embed to attach to the message.</param>
public struct CommandHelpMessage(string? content = null, DiscordEmbed? embed = null)
{
    /// <summary>
    /// Gets the contents of the help message.
    /// </summary>
    public string? Content { get; } = content;

    /// <summary>
    /// Gets the embed attached to the help message.
    /// </summary>
    public DiscordEmbed? Embed { get; } = embed;
}
