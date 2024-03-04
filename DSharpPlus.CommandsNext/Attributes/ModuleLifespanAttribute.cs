using System;

namespace DSharpPlus.CommandsNext.Attributes;

/// <summary>
/// Defines a lifespan for this command module.
/// </summary>
/// <remarks>
/// Defines a lifespan for this command module.
/// </remarks>
/// <param name="lifespan">Lifespan for this module.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ModuleLifespanAttribute(ModuleLifespan lifespan) : Attribute
{
    /// <summary>
    /// Gets the lifespan defined for this module.
    /// </summary>
    public ModuleLifespan Lifespan { get; } = lifespan;
}

/// <summary>
/// Defines lifespan of a command module.
/// </summary>
public enum ModuleLifespan : int
{
    /// <summary>
    /// Defines that this module will be instantiated once.
    /// </summary>
    Singleton = 0,

    /// <summary>
    /// Defines that this module will be instantiated every time a containing command is called.
    /// </summary>
    Transient = 1
}
