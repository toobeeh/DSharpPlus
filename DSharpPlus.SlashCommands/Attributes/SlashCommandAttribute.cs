using System;

namespace DSharpPlus.SlashCommands;

/// <summary>
/// Marks this method as a slash command.
/// </summary>
/// <remarks>
/// Marks this method as a slash command.
/// </remarks>
/// <param name="name">Sets the name of this slash command.</param>
/// <param name="description">Sets the description of this slash command.</param>
/// <param name="defaultPermission">Sets whether the command should be enabled by default.</param>
/// <param name="nsfw">Sets whether the command is age restricted.</param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class SlashCommandAttribute(string name, string description, bool defaultPermission = true, bool nsfw = false) : Attribute
{
    /// <summary>
    /// Gets the name of this command.
    /// </summary>
    public string Name { get; } = name.ToLower();

    /// <summary>
    /// Gets the description of this command.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// Gets whether this command is enabled by default.
    /// </summary>
    public bool DefaultPermission { get; } = defaultPermission;

    /// <summary>
    /// Gets whether this command is age restricted.
    /// </summary>
    public bool NSFW { get; } = nsfw;
}
