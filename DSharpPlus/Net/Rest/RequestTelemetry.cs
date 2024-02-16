namespace DSharpPlus.Net;

using System;

public struct RequestTelemetry
{
    internal DateTime LastUpdate { get; set; }
    internal int TotalDelay { get; set; }
    internal int DelayCount { get; set; }
}
