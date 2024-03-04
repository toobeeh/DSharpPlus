using System;

namespace DSharpPlus.SlashCommands;

/// <summary>
/// Marks this class a slash command group.
/// </summary>
/// <remarks>
/// Marks this class as a slash command group.
/// </remarks>
/// <param name="name">Sets the name of this command group.</param>
/// <param name="description">Sets the description of this command group.</param>
/// <param name="defaultPermission">Sets whether this command group is enabled on default.</param>
/// <param name="nsfw">Sets whether the command group is age restricted.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class SlashCommandGroupAttribute(string name, string description, bool defaultPermission = true, bool nsfw = false) : Attribute
{
    /// <summary>
    /// Gets the name of this slash command group.
    /// </summary>
    public string Name { get; } = name.ToLower();

    /// <summary>
    /// Gets the description of this slash command group.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// Gets whether this command is enabled on default.
    /// </summary>
    public bool DefaultPermission { get; } = defaultPermission;

    /// <summary>
    /// Gets whether this command is age restricted.
    /// </summary>
    public bool NSFW { get; } = nsfw;
}
