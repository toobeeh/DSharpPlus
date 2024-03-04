using System;
using System.Reflection;

namespace DSharpPlus.CommandsNext.Exceptions;

/// <summary>
/// Thrown when the command service fails to build a command due to a problem with its overload.
/// </summary>
/// <remarks>
/// Creates a new <see cref="InvalidOverloadException"/>.
/// </remarks>
/// <param name="message">Exception message.</param>
/// <param name="method">Method that caused the problem.</param>
/// <param name="parameter">Method argument that caused the problem.</param>
public sealed class InvalidOverloadException(string message, MethodInfo method, ParameterInfo? parameter) : Exception(message)
{
    /// <summary>
    /// Gets the method that caused this exception.
    /// </summary>
    public MethodInfo Method { get; } = method;

    /// <summary>
    /// Gets or sets the argument that caused the problem. This can be null.
    /// </summary>
    public ParameterInfo? Parameter { get; } = parameter;

    /// <summary>
    /// Creates a new <see cref="InvalidOverloadException"/>.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="method">Method that caused the problem.</param>
    public InvalidOverloadException(string message, MethodInfo method)
        : this(message, method, null)
    { }

    /// <summary>
    /// Returns a string representation of this <see cref="InvalidOverloadException"/>.
    /// </summary>
    /// <returns>A string representation.</returns>
    public override string ToString()
    {
        // much like System.ArgumentNullException works
        return this.Parameter == null
            ? $"{this.GetType()}: {this.Message}\nMethod: {this.Method} (declared in {this.Method.DeclaringType})"
            : $"{this.GetType()}: {this.Message}\nMethod: {this.Method} (declared in {this.Method.DeclaringType})\nArgument: {this.Parameter.ParameterType} {this.Parameter.Name}";
    }
}
