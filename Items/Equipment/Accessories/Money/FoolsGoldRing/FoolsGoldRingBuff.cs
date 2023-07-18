using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Money.FoolsGoldRing;

public class FoolsGoldRingBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}