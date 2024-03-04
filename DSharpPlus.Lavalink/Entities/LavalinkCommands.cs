using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DSharpPlus.Lavalink.Entities;

internal sealed class LavalinkConfigureResume(string key, int timeout) : LavalinkPayload("configureResuming")
{
    [JsonProperty("key")]
    public string Key { get; } = key;

    [JsonProperty("timeout")]
    public int Timeout { get; } = timeout;
}

internal sealed class LavalinkDestroy(LavalinkGuildConnection lvl) : LavalinkPayload("destroy", lvl.GuildIdString)
{
}

internal sealed class LavalinkPlay(LavalinkGuildConnection lvl, LavalinkTrack track) : LavalinkPayload("play", lvl.GuildIdString)
{
    [JsonProperty("track")]
    public string Track { get; } = track.TrackString;
}

internal sealed class LavalinkPlayPartial(LavalinkGuildConnection lvl, LavalinkTrack track, TimeSpan start, TimeSpan stop) : LavalinkPayload("play", lvl.GuildIdString)
{
    [JsonProperty("track")]
    public string Track { get; } = track.TrackString;

    [JsonProperty("startTime")]
    public long StartTime { get; } = (long)start.TotalMilliseconds;

    [JsonProperty("endTime")]
    public long StopTime { get; } = (long)stop.TotalMilliseconds;
}

internal sealed class LavalinkPause(LavalinkGuildConnection lvl, bool pause) : LavalinkPayload("pause", lvl.GuildIdString)
{
    [JsonProperty("pause")]
    public bool Pause { get; } = pause;
}

internal sealed class LavalinkStop(LavalinkGuildConnection lvl) : LavalinkPayload("stop", lvl.GuildIdString)
{
}

internal sealed class LavalinkSeek(LavalinkGuildConnection lvl, TimeSpan position) : LavalinkPayload("seek", lvl.GuildIdString)
{
    [JsonProperty("position")]
    public long Position { get; } = (long)position.TotalMilliseconds;
}

internal sealed class LavalinkVolume(LavalinkGuildConnection lvl, int volume) : LavalinkPayload("volume", lvl.GuildIdString)
{
    [JsonProperty("volume")]
    public int Volume { get; } = volume;
}

internal sealed class LavalinkEqualizer(LavalinkGuildConnection lvl, IEnumerable<LavalinkBandAdjustment> bands) : LavalinkPayload("equalizer", lvl.GuildIdString)
{
    [JsonProperty("bands")]
    public IEnumerable<LavalinkBandAdjustment> Bands { get; } = bands;
}
