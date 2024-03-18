namespace DSharpPlus.Commands.Processors.SlashCommands.Attributes;

using System;
using System.Collections.Generic;

/// <summary>
/// Specifies the allowed interaction contexts for a command.
/// </summary>
public sealed class SlashAllowedContextsAttribute(params InteractionContextType[] allowedContexts) : Attribute
{
    /// <summary>
    /// The contexts the command is allowed to be used in.
    /// </summary>
    public IReadOnlyList<InteractionContextType> AllowedContexts { get; } = allowedContexts;
}
