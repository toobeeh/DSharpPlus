// This Source Code form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Remora.Rest.Core;

using DSharpPlus.Core.Abstractions.Models;

namespace DSharpPlus.Core.Models;

/// <inheritdoc cref="IAuditLogChange" />
public sealed record AuditLogChange : IAuditLogChange
{
    /// <inheritdoc/>
    public Optional<string> NewValue { get; init; }

    /// <inheritdoc/>
    public Optional<string> OldValue { get; init; }

    /// <inheritdoc/>
    public required string Key { get; init; }
}