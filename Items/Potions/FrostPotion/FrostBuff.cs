using Aequus.Common.Buffs;

namespace Aequus.Items.Potions.FrostPotion;

public class FrostBuff : ModBuff {
    public override void SetStaticDefaults() {
        AequusBuff.AddPotionConflict(Type, BuffID.Warmth);
    }

    public override void Update(Player player, ref int buffIndex) {
        player.Aequus().potionFrost = true;
    }
}