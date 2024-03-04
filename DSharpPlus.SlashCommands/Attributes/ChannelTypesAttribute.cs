using System;
using System.Collections.Generic;

namespace DSharpPlus.SlashCommands;

/// <summary>
/// Defines allowed channel types for a channel parameter.
/// </summary>
/// <remarks>
/// Defines allowed channel types for a channel parameter.
/// </remarks>
/// <param name="channelTypes">The channel types to allow.</param>
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
public class ChannelTypesAttribute(params ChannelType[] channelTypes) : Attribute
{
    /// <summary>
    /// Allowed channel types.
    /// </summary>
    public IEnumerable<ChannelType> ChannelTypes { get; } = channelTypes;
}
