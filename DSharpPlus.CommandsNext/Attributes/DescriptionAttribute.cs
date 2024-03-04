using System;

namespace DSharpPlus.CommandsNext.Attributes;

/// <summary>
/// Gives this command, group, or argument a description, which is used when listing help.
/// </summary>
/// <remarks>
/// Gives this command, group, or argument a description, which is used when listing help.
/// </remarks>
/// <param name="description"></param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class DescriptionAttribute(string description) : Attribute
{
    /// <summary>
    /// Gets the description for this command, group, or argument.
    /// </summary>
    public string Description { get; } = description;
}
