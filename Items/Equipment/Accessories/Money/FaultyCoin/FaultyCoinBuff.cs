using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Money.FaultyCoin;

public class FaultyCoinBuff : ModBuff {
    public override void SetStaticDefaults() {
        Main.buffNoTimeDisplay[Type] = true;
        Main.buffNoSave[Type] = true;
    }

    public override bool RightClick(int buffIndex) {
        return false;
    }
}