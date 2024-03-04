using System;

namespace DSharpPlus.SlashCommands;

/// <summary>
/// Defines this slash command module's lifespan. Module lifespans are transient by default.
/// </summary>
/// <remarks>
/// Defines this slash command module's lifespan.
/// </remarks>
/// <param name="lifespan">The lifespan of the module. Module lifespans are transient by default.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class SlashModuleLifespanAttribute(SlashModuleLifespan lifespan) : Attribute
{
    /// <summary>
    /// Gets the lifespan.
    /// </summary>
    public SlashModuleLifespan Lifespan { get; } = lifespan;
}

/// <summary>
/// Represents a slash command module lifespan.
/// </summary>
public enum SlashModuleLifespan
{
    /// <summary>
    /// Whether this module should be initiated every time a command is run, with dependencies injected from a scope.
    /// </summary>
    Scoped,

    /// <summary>
    /// Whether this module should be initiated every time a command is run.
    /// </summary>
    Transient,

    /// <summary>
    /// Whether this module should be initiated at startup.
    /// </summary>
    Singleton
}
