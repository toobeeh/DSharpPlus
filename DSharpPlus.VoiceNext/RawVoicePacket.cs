using System;

namespace DSharpPlus.VoiceNext;

internal readonly struct RawVoicePacket(Memory<byte> bytes, int duration, bool silence)
{
    public RawVoicePacket(Memory<byte> bytes, int duration, bool silence, byte[] rentedBuffer)
        : this(bytes, duration, silence) => this.RentedBuffer = rentedBuffer;

    public readonly Memory<byte> Bytes = bytes;
    public readonly int Duration = duration;
    public readonly bool Silence = silence;

    public readonly byte[] RentedBuffer = null;
}
