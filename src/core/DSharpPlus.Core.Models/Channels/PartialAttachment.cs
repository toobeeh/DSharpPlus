// This Source Code form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

using DSharpPlus.Entities;

using Remora.Rest.Core;

using DSharpPlus.Core.Abstractions.Models;

namespace DSharpPlus.Core.Models;

/// <inheritdoc cref="IPartialAttachment" />
public sealed record PartialAttachment : IPartialAttachment
{
    /// <inheritdoc/>
    public Optional<Snowflake> Id { get; init; }

    /// <inheritdoc/>
    public Optional<string> Filename { get; init; }

    /// <inheritdoc/>
    public Optional<string> Description { get; init; }

    /// <inheritdoc/>
    public Optional<string> ContentType { get; init; }

    /// <inheritdoc/>
    public Optional<int> Size { get; init; }

    /// <inheritdoc/>
    public Optional<string> Url { get; init; }

    /// <inheritdoc/>
    public Optional<string> ProxyUrl { get; init; }

    /// <inheritdoc/>
    public Optional<int?> Height { get; init; }

    /// <inheritdoc/>
    public Optional<int?> Width { get; init; }

    /// <inheritdoc/>
    public Optional<bool> Ephemeral { get; init; }

    /// <inheritdoc/>
    public Optional<float> DurationSecs { get; init; }

    /// <inheritdoc/>
    public Optional<ReadOnlyMemory<byte>> Waveform { get; init; }

    /// <inheritdoc/>
    public Optional<DiscordAttachmentFlags> Flags { get; init; }
}