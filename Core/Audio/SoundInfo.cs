using Microsoft.Xna.Framework.Audio;

namespace AequusRemake.Core.Audio;

public readonly struct SoundInfo {
    public readonly byte[] Buffer;
    public readonly int SampleRate;
    public readonly AudioChannels Channels;
}
