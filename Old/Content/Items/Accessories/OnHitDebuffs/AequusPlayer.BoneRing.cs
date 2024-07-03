using Aequu2.Core.CodeGeneration;
using Aequu2.Old.Content.Items.Accessories.OnHitDebuffs;

namespace Aequu2;

public partial class Aequu2Player {
    [ResetEffects]
    public int accBoneRing;

    private void ProcBoneRing(NPC target) {
        if (accBoneRing > 0) {
            target.AddBuff(ModContent.BuffType<BoneRingDebuff>(), BoneRing.DebuffDuration * accBoneRing);
        }
    }
}