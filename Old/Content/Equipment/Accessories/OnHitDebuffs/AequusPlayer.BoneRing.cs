using Aequus.Core.CodeGeneration;
using Aequus.Old.Content.Equipment.Accessories.OnHitDebuffs;

namespace Aequus;

public partial class AequusPlayer {
    [ResetEffects]
    public int accBoneRing;

    private void ProcBoneRing(NPC target) {
        if (accBoneRing > 0) {
            target.AddBuff(ModContent.BuffType<BoneRingDebuff>(), BoneRing.DebuffDuration * accBoneRing);
        }
    }
}