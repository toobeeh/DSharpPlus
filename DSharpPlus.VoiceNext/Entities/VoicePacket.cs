using System;

namespace DSharpPlus.VoiceNext.Entities;

internal struct VoicePacket(ReadOnlyMemory<byte> bytes, int msDuration, bool isSilence = false)
{
    public ReadOnlyMemory<byte> Bytes { get; } = bytes;
    public int MillisecondDuration { get; } = msDuration;
    public bool IsSilence { get; set; } = isSilence;
}
