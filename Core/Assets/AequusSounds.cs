using Terraria.Audio;

namespace Aequus;

public sealed partial class AequusSounds : ILoadable {
    private static SoundStyle GetMultisound(SoundStyle zero, int variants) {
        return new(zero.SoundPath[..^1], 0, variants);
    }

    public static SoundStyle ShardHit { get; private set; }
    public static SoundStyle JunkJetShoot { get; private set; }
    public static SoundStyle SwordHit { get; private set; }
    public static SoundStyle HeavySwing { get; private set; }
    public static SoundStyle ConductiveBlock { get; private set; }
    public static SoundStyle ConductiveBlockPlaced { get; private set; }
    public static SoundStyle ChainedSoulAttackExplode { get; private set; }

    public void Load(Mod mod) {
        ChainedSoulAttackExplode = GetMultisound(ChainedSoulAttackExplode0, 3) with { Pitch = 0f, PitchVariance = 0.1f, MaxInstances = 3 };
        ShardHit = GetMultisound(ShardHit0, 2) with { Volume = 0.5f, Pitch = -0.2f, PitchVariance = 0.15f, };
        JunkJetShoot = GetMultisound(JunkJetShoot0, 2) with { Volume = 0.66f, PitchVariance = 0.15f, MaxInstances = 3, };
        SwordHit = GetMultisound(SwordHit0, 4) with { Volume = 0.7f, PitchVariance = 0.1f, MaxInstances = 3 };
        HeavySwing = GetMultisound(HeavySwing0, 6) with { Volume = 0.7f, PitchVariance = 0.1f, MaxInstances = 3 };
        ConductiveBlock = GetMultisound(ConductiveBlock0, 4) with { Pitch = 0.6f, PitchVariance = 0.05f };
        ConductiveBlockPlaced = GetMultisound(ConductiveBlock0, 4);
    }

    public void Unload() {
    }
}