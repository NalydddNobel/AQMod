using Terraria.Audio;

namespace Aequus.Core.Assets;

public record SoundAsset {
    private SoundStyle sound;
    public readonly string Path;
    public readonly string ModPath;
    public SoundStyle Sound => sound;

    public float Volume;
    public float Pitch;
    public float PitchVariance;

    public SoundAsset(string path, int amount = 1) {
        Path = path;
        ModPath = path[7..];
        Volume = 1f;
        Pitch = 0f;
        PitchVariance = 0f;
        if (amount <= 1) {
            sound = new(path);
        }
        else {
            sound = new(path, 0, amount);
        }
    }

    internal void Unload() {
        sound = default;
    }

    public static implicit operator SoundStyle(SoundAsset value) {
        return value.Sound with {
            Volume = value.Volume,
            Pitch = value.Pitch,
            PitchVariance = value.PitchVariance,
        };
    }
}