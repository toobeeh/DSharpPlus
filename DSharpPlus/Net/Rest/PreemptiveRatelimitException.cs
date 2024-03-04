namespace DSharpPlus.Net;

using System;
using System.Diagnostics.CodeAnalysis;

[method: SetsRequiredMembers]
internal class PreemptiveRatelimitException(string scope, TimeSpan resetAfter) : Exception
{
    public required string Scope { get; set; } = scope;

    public required TimeSpan ResetAfter { get; set; } = resetAfter;
}
