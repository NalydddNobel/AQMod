using Terraria.Audio;

namespace Aequus;

public sealed partial class AequusSounds : ILoad {
    private static SoundStyle GetMultisound(SoundStyle zero, int variants) {
        return new(zero.SoundPath[..^1], 0, variants);
    }

    public static SoundStyle PollutedOcean_AmbientB { get; private set; }
    public static SoundStyle PollutedOcean_AmbientA { get; private set; }
    public static SoundStyle ShardHit { get; private set; }
    public static SoundStyle JunkJetShoot { get; private set; }
    public static SoundStyle SwordHit { get; private set; }
    public static SoundStyle HeavySwing { get; private set; }
    public static SoundStyle ScrapBlockBreak { get; private set; }
    public static SoundStyle ScrapBlockPlaced { get; private set; }
    public static SoundStyle ConductiveBlockBreak { get; private set; }
    public static SoundStyle ConductiveBlockPlaced { get; private set; }
    public static SoundStyle ChainedSoulAttackExplode { get; private set; }

    public void Load(Mod mod) {
        PollutedOcean_AmbientB = GetMultisound(PollutedOcean_AmbientB0, 2) with { Pitch = 0f, PitchVariance = 0.1f, MaxInstances = 1, Type = SoundType.Ambient };
        PollutedOcean_AmbientA = GetMultisound(PollutedOcean_AmbientA0, 3) with { Pitch = 0f, PitchVariance = 0.1f, MaxInstances = 1, Type = SoundType.Ambient };
        ChainedSoulAttackExplode = GetMultisound(ChainedSoulAttackExplode0, 3) with { Pitch = 0f, PitchVariance = 0.1f, MaxInstances = 3 };
        ShardHit = GetMultisound(ShardHit0, 2) with { Volume = 0.5f, Pitch = -0.2f, PitchVariance = 0.15f, };
        JunkJetShoot = GetMultisound(JunkJetShoot0, 2) with { Volume = 0.66f, PitchVariance = 0.15f, MaxInstances = 3, };
        SwordHit = GetMultisound(SwordHit0, 4) with { Volume = 0.7f, PitchVariance = 0.1f, MaxInstances = 3 };
        HeavySwing = GetMultisound(HeavySwing0, 6) with { Volume = 0.7f, PitchVariance = 0.1f, MaxInstances = 3 };
        ConductiveBlockBreak = GetMultisound(ConductiveBlock0, 4);
        ConductiveBlockPlaced = ConductiveBlockBreak with { Pitch = 0.6f, PitchVariance = 0.05f };
        ScrapBlockBreak = GetMultisound(ScrapBlock0, 4);
        ScrapBlockPlaced = ScrapBlockBreak with { Pitch = 0.3f, PitchVariance = 0.05f };
    }

    public void Unload() {
    }
}