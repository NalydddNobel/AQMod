using Aequus.Common.Buffs;

namespace Aequus.Buffs;
public class ManathirstBuff : ModBuff {
    public override void SetStaticDefaults() {
        AequusBuff.AddPotionConflict(Type, BuffID.ManaRegeneration);
        AequusBuff.AddPotionConflict(Type, ModContent.BuffType<BloodthirstBuff>());
    }
}