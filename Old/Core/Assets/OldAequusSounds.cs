using Terraria.Audio;

namespace Aequus.Old.Core.Assets;

public class OldAequusSounds : ILoad {
    public static SoundStyle HighSteaks { get; private set; }
    public static SoundStyle Meathook { get; private set; }

    public void Load(Mod mod) {
        HighSteaks = new SoundStyle(AequusSounds.HighSteaks0.SoundPath[..^1], 0, 2) with { Volume = 0.2f, PitchVariance = 0.2f };
        Meathook = new SoundStyle(AequusSounds.Meathook0.SoundPath[..^1], 0, 2) with { Volume = 0.6f, PitchVariance = 0.1f, MaxInstances = 8, };
    }

    public void Unload() {
        HighSteaks = default;
    }
}