using System;

namespace DSharpPlus.SlashCommands;

/// <summary>
/// Sets the name for this enum choice.
/// </summary>
/// <remarks>
/// Sets the name for this enum choice.
/// </remarks>
/// <param name="name">The name for this enum choice.</param>
[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
public sealed class ChoiceNameAttribute(string name) : Attribute
{
    /// <summary>
    /// The name.
    /// </summary>
    public string Name { get; } = name;
}
