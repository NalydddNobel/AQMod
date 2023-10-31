using Terraria.Audio;

namespace Aequus;

public partial class AequusSounds {
    private static SoundStyle GetMultisound(SoundStyle zero, int variants) {
        return new(zero.SoundPath[..^1], 0, variants - 1);
    }

    public static readonly SoundStyle SwordHit = GetMultisound(SwordHit0, 4) with { Volume = 0.7f, PitchVariance = 0.1f, MaxInstances = 3 };
    public static readonly SoundStyle HeavySwing = GetMultisound(HeavySwing0, 6) with { Volume = 0.7f, PitchVariance = 0.1f, MaxInstances = 3 };
    public static readonly SoundStyle ConductiveBlock = GetMultisound(ConductiveBlock0, 4);
    public static readonly SoundStyle ConductiveBlockPlaced = GetMultisound(ConductiveBlock0, 4) with { Pitch = 0.6f, PitchVariance = 0.05f };
}