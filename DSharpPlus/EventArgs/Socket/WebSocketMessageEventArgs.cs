using DSharpPlus.AsyncEvents;

namespace DSharpPlus.EventArgs;

/// <summary>
/// Represents base class for raw socket message event arguments.
/// </summary>
public abstract class SocketMessageEventArgs : AsyncEventArgs
{ }

/// <summary>
/// Represents arguments for text message websocket event.
/// </summary>
/// <remarks>
/// Creates a new instance of text message event arguments.
/// </remarks>
/// <param name="message">Received message string.</param>
public sealed class SocketTextMessageEventArgs(string message) : SocketMessageEventArgs
{
    /// <summary>
    /// Gets the received message string.
    /// </summary>
    public string Message { get; } = message;
}

/// <summary>
/// Represents arguments for binary message websocket event.
/// </summary>
/// <remarks>
/// Creates a new instance of binary message event arguments.
/// </remarks>
/// <param name="message">Received message bytes.</param>
public sealed class SocketBinaryMessageEventArgs(byte[] message) : SocketMessageEventArgs
{
    /// <summary>
    /// Gets the received message bytes.
    /// </summary>
    public byte[] Message { get; } = message;
}
