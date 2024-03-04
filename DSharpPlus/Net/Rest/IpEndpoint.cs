using System.Net;

namespace DSharpPlus.Net;

/// <summary>
/// Represents a network connection IP endpoint.
/// </summary>
/// <remarks>
/// Creates a new IP endpoint structure.
/// </remarks>
/// <param name="address">IP address to connect to.</param>
/// <param name="port">Port to use for connection.</param>
public struct IpEndpoint(IPAddress address, int port)
{
    /// <summary>
    /// Gets or sets the hostname associated with this endpoint.
    /// </summary>
    public IPAddress Address { get; set; } = address;

    /// <summary>
    /// Gets or sets the port associated with this endpoint.
    /// </summary>
    public int Port { get; set; } = port;
}
