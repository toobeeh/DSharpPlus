namespace DSharpPlus.Commands;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.Commands.ContextChecks;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.Commands.Exceptions;
using DSharpPlus.Commands.Invocation;
using DSharpPlus.Commands.Trees;
using Microsoft.Extensions.DependencyInjection;

public class DefaultCommandExecutor : ICommandExecutor
{
    /// <summary>
    /// This dictionary contains all of the command wrappers intended to be used for bypassing the overhead of reflection and Task/ValueTask handling.
    /// </summary>
    protected readonly ConcurrentDictionary<Ulid, Func<object?, object?[], ValueTask>> _commandWrappers = new();

    /// <inheritdoc/>
    [SuppressMessage("Quality", "CA2012", Justification = "The worker does not pool instances and has its own error handling.")]
    public virtual async ValueTask ExecuteAsync(CommandContext context, bool awaitCommandExecution = false, CancellationToken cancellationToken = default)
    {
        if (!awaitCommandExecution)
        {
            _ = WorkerAsync(context);
        }
        else
        {
            await WorkerAsync(context);
        }
    }

    /// <summary>
    /// This method will execute the command provided without any safety checks, context checks or event invocation.
    /// </summary>
    /// <param name="context">The context of the command being executed.</param>
    /// <returns>A tuple containing the command object and any error that occurred during execution. The command object may be null when the delegate is static and is from a static class.</returns>
    public virtual async ValueTask<(object? CommandObject, Exception? Error)> UnconditionallyExecuteAsync(CommandContext context)
    {
        // Keep the command object in scope so it can be accessed after the command has been executed.
        object? commandObject = null;

        try
        {
            // If the class isn't static, we need to create an instance of it.
            if (!context.Command.Method!.DeclaringType!.IsAbstract || !context.Command.Method.DeclaringType.IsSealed)
            {
                // The delegate's object was provided, so we can use that.
                commandObject = context.Command.Target is not null
                    ? context.Command.Target
                    : ActivatorUtilities.CreateInstance(context.ServiceProvider, context.Command.Method.DeclaringType);
            }

            // Grab the method that wraps Task/ValueTask execution.
            if (!this._commandWrappers.TryGetValue(context.Command.Id, out Func<object?, object?[], ValueTask>? wrapper))
            {
                wrapper = CommandEmitUtil.GetCommandInvocationFunc(context.Command.Method, context.Command.Target);
                this._commandWrappers[context.Command.Id] = wrapper;
            }

            // Execute the command and return the result.
            await wrapper(commandObject, [context, .. context.Arguments.Values]);
            return (commandObject, null);
        }
        catch (Exception error)
        {
            // The command threw. Trim down the stack trace as much as we can to provide helpful information to the developer.
            if (error is TargetInvocationException targetInvocationError && targetInvocationError.InnerException is not null)
            {
                error = ExceptionDispatchInfo.Capture(targetInvocationError.InnerException).SourceException;
            }

            return (commandObject, error);
        }
    }

    /// <summary>
    /// Ensures the command is executable before attempting to execute it.
    /// </summary>
    /// <remarks>
    /// This does NOT execute any context checks. This only checks if the command is executable based on the number of arguments provided.
    /// </remarks>
    /// <param name="context">The context of the command being executed.</param>
    /// <param name="errorMessage">Any error message that occurred during the check.</param>
    /// <returns>Whether the command can be executed.</returns>
    protected virtual bool IsCommandExecutable(CommandContext context, [NotNullWhen(false)] out string? errorMessage)
    {
        if (context.Command.Method is null)
        {
            errorMessage = "Unable to execute a command that has no method. Is this command a group command?";
            return false;
        }
        else if (context.Command.Target is null && context.Command.Method.DeclaringType is null)
        {
            errorMessage = "Unable to execute a delegate that has no target or declaring type. Is this command a group command?";
            return false;
        }
        else if (context.Arguments.Count != context.Command.Parameters.Count)
        {
            errorMessage = "The number of arguments provided does not match the number of parameters the command expects.";
            return false;
        }

        errorMessage = null;
        return true;
    }

    protected virtual async ValueTask<IReadOnlyList<ContextCheckFailedData>> ExecuteContextChecksAsync(CommandContext context)
    {
        // Add all of the checks attached to the delegate first.
        List<ContextCheckAttribute> checks = new(context.Command.Attributes.OfType<ContextCheckAttribute>());

        // Add the parent's checks last so we can execute the checks in order.
        Command? parent = context.Command.Parent;
        while (parent is not null)
        {
            checks.AddRange(parent.Attributes.OfType<ContextCheckAttribute>());
            parent = parent.Parent;
        }

        // If there are no checks, we can skip this step.
        if (checks.Count == 0)
        {
            return [];
        }

        // Execute all checks and return any that failed.
        List<ContextCheckFailedData> failedChecks = [];

        // Reuse the same instance of UnconditionalCheckAttribute for all unconditional checks.
        UnconditionalCheckAttribute unconditionalCheck = new();

        // First, execute all unconditional checks
        foreach (ContextCheckMapEntry entry in context.Extension.Checks)
        {
            // Users must implement the check that requests the UnconditionalCheckAttribute from IContextCheck<UnconditionalCheckAttribute>
            if (entry.AttributeType != typeof(UnconditionalCheckAttribute))
            {
                continue;
            }

            try
            {
                // Create the check instance
                object check = ActivatorUtilities.CreateInstance(context.ServiceProvider, entry.CheckType);

                // Execute it
                string? result = await entry.ExecuteCheckAsync(check, unconditionalCheck, context);

                // It failed, add it to the list and continue with the others
                if (result is not null)
                {
                    failedChecks.Add(new()
                    {
                        ContextCheckAttribute = unconditionalCheck,
                        ErrorMessage = result
                    });
                }
            }
            catch (Exception error)
            {
                failedChecks.Add(new()
                {
                    ContextCheckAttribute = unconditionalCheck,
                    ErrorMessage = error.Message,
                    Exception = error
                });
            }
        }

        // Reverse foreach so we execute the top-most command's checks first.
        for (int i = checks.Count - 1; i >= 0; i--)
        {
            // Search for any checks that match the current check's type, as there can be multiple checks for the same attribute.
            foreach (ContextCheckMapEntry entry in context.Extension.Checks)
            {
                ContextCheckAttribute checkAttribute = checks[i];

                // Skip checks that don't match the current check's type.
                if (entry.AttributeType != checkAttribute.GetType())
                {
                    continue;
                }

                try
                {
                    // Create the check instance
                    object check = ActivatorUtilities.CreateInstance(context.ServiceProvider, entry.CheckType);

                    // Execute it
                    string? result = await entry.ExecuteCheckAsync(check, checkAttribute, context);

                    // It failed, add it to the list and continue with the others
                    if (result is not null)
                    {
                        failedChecks.Add(new()
                        {
                            ContextCheckAttribute = checkAttribute,
                            ErrorMessage = result
                        });

                        continue;
                    }
                }
                // try/catch blocks are free until they catch
                catch (Exception error)
                {
                    failedChecks.Add(new()
                    {
                        ContextCheckAttribute = checkAttribute,
                        ErrorMessage = error.Message,
                        Exception = error
                    });
                }
            }
        }

        return failedChecks;
    }

    /// <summary>
    /// Invokes the <see cref="CommandsExtension.CommandExecuted"/> event, which isn't normally exposed to the public API.
    /// </summary>
    /// <param name="extension">The extension/shard that the event is being invoked on.</param>
    /// <param name="eventArgs">The event arguments to pass to the event.</param>
    protected virtual async ValueTask InvokedCommandErroredEventAsync(CommandsExtension extension, CommandErroredEventArgs eventArgs) => await extension._commandErrored.InvokeAsync(extension, eventArgs);

    /// <summary>
    /// This method will ensure that the command is executable, execute all context checks, and then execute the command, and invoke the appropriate events.
    /// </summary>
    /// <remarks>
    /// This method - without exception - should never throw. If any errors were to occur, they will be delegated to the <see cref="CommandsExtension.CommandErrored"/> event.
    /// </remarks>
    /// <param name="context">The context of the command being executed.</param>
    protected virtual async ValueTask WorkerAsync(CommandContext context)
    {
        // Do some safety checks to ensure the command is both executable
        if (!IsCommandExecutable(context, out string? errorMessage))
        {
            await InvokedCommandErroredEventAsync(context.Extension, new CommandErroredEventArgs()
            {
                Context = context,
                Exception = new CommandNotExecutableException(context.Command, errorMessage),
                CommandObject = null
            });

            return;
        }

        // Execute all context checks and return any that failed.
        IReadOnlyList<ContextCheckFailedData> failedChecks = await ExecuteContextChecksAsync(context);
        if (failedChecks.Count > 0)
        {
            await InvokedCommandErroredEventAsync(context.Extension, new CommandErroredEventArgs()
            {
                Context = context,
                Exception = new ChecksFailedException(failedChecks, context.Command),
                CommandObject = null
            });

            return;
        }

        // Execute the command
        (object? commandObject, Exception? error) = await UnconditionallyExecuteAsync(context);

        // If the command threw an exception, invoke the CommandErrored event.
        if (error is not null)
        {
            await InvokedCommandErroredEventAsync(context.Extension, new CommandErroredEventArgs()
            {
                Context = context,
                Exception = error,
                CommandObject = commandObject
            });
        }
        // Otherwise, invoke the CommandExecuted event.
        else
        {
            await context.Extension._commandExecuted.InvokeAsync(context.Extension, new CommandExecutedEventArgs()
            {
                Context = context,
                CommandObject = commandObject
            });
        }

        // Dispose of the service scope if it was created.
        context.ServiceScope.Dispose();
    }
}