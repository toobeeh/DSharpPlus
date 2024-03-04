using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DSharpPlus.CommandsNext.Attributes;

namespace DSharpPlus.CommandsNext.Exceptions;

/// <summary>
/// Indicates that one or more checks for given command have failed.
/// </summary>
/// <remarks>
/// Creates a new <see cref="ChecksFailedException"/>.
/// </remarks>
/// <param name="command">Command that failed to execute.</param>
/// <param name="ctx">Context in which the command was executed.</param>
/// <param name="failedChecks">A collection of checks that failed.</param>
public class ChecksFailedException(Command command, CommandContext ctx, IEnumerable<CheckBaseAttribute> failedChecks) : Exception("One or more pre-execution checks failed.")
{
    /// <summary>
    /// Gets the command that was executed.
    /// </summary>
    public Command Command { get; } = command;

    /// <summary>
    /// Gets the context in which given command was executed.
    /// </summary>
    public CommandContext Context { get; } = ctx;

    /// <summary>
    /// Gets the checks that failed.
    /// </summary>
    public IReadOnlyList<CheckBaseAttribute> FailedChecks { get; } = new ReadOnlyCollection<CheckBaseAttribute>(new List<CheckBaseAttribute>(failedChecks));
}
