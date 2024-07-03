using Terraria.Audio;

namespace Aequu2;

public partial class Aequu2Sounds {
    public static SoundStyle RedSpriteThunderClap { get; private set; }
    public static SoundStyle HighSteaks { get; private set; }
    public static SoundStyle Meathook { get; private set; }

    private static void LoadOldSounds() {
        RedSpriteThunderClap = new SoundStyle(RedSpriteThunderClap0.SoundPath[..^1], 0, 1) with { PitchVariance = 0.1f };
        HighSteaks = new SoundStyle(HighSteaks0.SoundPath[..^1], 0, 2) with { Volume = 0.2f, PitchVariance = 0.2f };
        Meathook = new SoundStyle(Meathook0.SoundPath[..^1], 0, 2) with { Volume = 0.6f, PitchVariance = 0.1f, MaxInstances = 8, };
    }
}