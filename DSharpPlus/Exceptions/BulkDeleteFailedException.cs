namespace DSharpPlus.Exceptions;

using System;

public class BulkDeleteFailedException(int messagesDeleted, Exception innerException) : Exception("Failed to delete all messages. See inner exception", innerException: innerException)
{

    /// <summary>
    /// Number of messages that were deleted successfully.
    /// </summary>
    public int MessagesDeleted { get; init; } = messagesDeleted;
}
