namespace DSharpPlus.Net.Ratelimiting;

using System;

/// <summary>
/// A value-type variant of <seealso cref="RateLimitBucket"/> for extraction, in case we don't need the object.
/// </summary>
internal readonly record struct RatelimitCandidateBucket(int Maximum, int Remaining, DateTime Reset)
{
    public RatelimitBucket ToFullBucket()
        => new(this.Maximum, this.Remaining, this.Reset);
}
