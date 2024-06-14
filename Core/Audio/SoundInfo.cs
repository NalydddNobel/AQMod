using Microsoft.Xna.Framework.Audio;

namespace Aequus.Core.Audio;

public readonly struct SoundInfo {
    public readonly byte[] Buffer;
    public readonly int SampleRate;
    public readonly AudioChannels Channels;
}
