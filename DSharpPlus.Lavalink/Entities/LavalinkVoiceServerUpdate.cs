using System.Globalization;
using DSharpPlus.EventArgs;
using Newtonsoft.Json;

namespace DSharpPlus.Lavalink.Entities;

internal sealed class LavalinkVoiceServerUpdate
{
    [JsonProperty("token")]
    public string Token { get; }

    [JsonProperty("guild_id")]
    public string GuildId { get; }

    [JsonProperty("endpoint")]
    public string Endpoint { get; }

    internal LavalinkVoiceServerUpdate(VoiceServerUpdateEventArgs vsu)
    {
        this.Token = vsu.VoiceToken;
        this.GuildId = vsu.Guild.Id.ToString(CultureInfo.InvariantCulture);
        this.Endpoint = vsu.Endpoint;
    }
}

internal sealed class LavalinkVoiceUpdate(VoiceStateUpdateEventArgs vstu, VoiceServerUpdateEventArgs vsrvu) : LavalinkPayload("voiceUpdate", vstu.Guild.Id.ToString(CultureInfo.InvariantCulture))
{
    [JsonProperty("sessionId")]
    public string SessionId { get; } = vstu.SessionId;

    [JsonProperty("event")]
    internal LavalinkVoiceServerUpdate Event { get; } = new LavalinkVoiceServerUpdate(vsrvu);
}
