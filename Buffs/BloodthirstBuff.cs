using Aequus.Common.Buffs;

namespace Aequus.Buffs;
public class BloodthirstBuff : ModBuff {
    public override void SetStaticDefaults() {
        AequusBuff.AddPotionConflict(Type, BuffID.Heartreach);
    }
}