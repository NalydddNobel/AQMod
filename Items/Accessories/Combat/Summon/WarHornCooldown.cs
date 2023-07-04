using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Combat.Summon; 

public class WarHornCooldown : ModBuff {
    public override void SetStaticDefaults() {
        Main.debuff[Type] = true;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.Aequus().cooldownWarHorn = true;
    }
}