namespace DSharpPlus.Net.Ratelimiting;

using System;
using System.Threading.Tasks;

internal readonly record struct RatelimitBucketLease(RatelimitBucketContainer container, int index) : IAsyncDisposable
{
    public ValueTask DisposeAsync() => throw new NotImplementedException();
}
