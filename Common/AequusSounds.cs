using Aequus.Common.Structures;
using System;
using Terraria.Audio;

namespace Aequus;

public sealed partial class AequusSounds {
    public static readonly ImplicitLazy<SoundStyle> ThunderClap = NewMultisound(() => thunderClap0, 2);
    public static readonly ImplicitLazy<SoundStyle> CoinHit = NewMultisound(() => coinHit0, 2);
    public static readonly ImplicitLazy<SoundStyle> ElectricHit = NewMultisound(() => electricHit0, 2);
    public static readonly ImplicitLazy<SoundStyle> OmegaStariteHit = NewMultisound(() => OmegaStariteHit0, 3);
    public static readonly ImplicitLazy<SoundStyle> UmystickShoot = NewMultisound(() => UmystickShoot0, 3);
    public static readonly ImplicitLazy<SoundStyle> UmystickJump = NewMultisound(() => UmystickJump0, 2);
    public static readonly ImplicitLazy<SoundStyle> UmystickBreak = NewMultisound(() => UmystickBreak0, 4);
    public static readonly ImplicitLazy<SoundStyle> LargeSwordSlash = NewMultisound(() => LargeSwordSlash0, 2);
    public static readonly ImplicitLazy<SoundStyle> SwordSwipe = NewMultisound(() => SwordSwipe0, 6);
    public static readonly ImplicitLazy<SoundStyle> SwordHit = NewMultisound(() => SwordHit0, 4);
    public static readonly ImplicitLazy<SoundStyle> MeathookDamageBonusProc = NewMultisound(() => MeathookDamageBonusProc0, 2);
    public static readonly ImplicitLazy<SoundStyle> PossessedShardHit = NewMultisound(() => PossessedShardHit0, 2);
    public static readonly ImplicitLazy<SoundStyle> JunkJetShoot = NewMultisound(() => JunkJetShoot0, 2);

    private static ImplicitLazy<SoundStyle> NewMultisound(Func<SoundStyle> getFirstSoundFactory, int variantCount) {
        return new ImplicitLazy<SoundStyle>(() => new(getFirstSoundFactory().SoundPath[..^1], 0, variantCount));
    }
}