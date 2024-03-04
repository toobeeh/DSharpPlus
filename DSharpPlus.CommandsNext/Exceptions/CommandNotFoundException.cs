using System;

namespace DSharpPlus.CommandsNext.Exceptions;

/// <summary>
/// Thrown when the command service fails to find a command.
/// </summary>
/// <remarks>
/// Creates a new <see cref="CommandNotFoundException"/>.
/// </remarks>
/// <param name="command">Name of the command that was not found.</param>
public sealed class CommandNotFoundException(string command) : Exception("Specified command was not found.")
{
    /// <summary>
    /// Gets the name of the command that was not found.
    /// </summary>
    public string CommandName { get; set; } = command;

    /// <summary>
    /// Returns a string representation of this <see cref="CommandNotFoundException"/>.
    /// </summary>
    /// <returns>A string representation.</returns>
    public override string ToString() => $"{this.GetType()}: {this.Message}\nCommand name: {this.CommandName}"; // much like System.ArgumentNullException works
}
