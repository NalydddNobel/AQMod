using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.UltimateSword; 

public class UltimateSwordBuff : ModBuff {
    public override bool RightClick(int buffIndex) {
        return false;
    }

    public override void Update(Player player, ref int buffIndex) {
        player.statDefense += 8;
        player.noKnockback = true;
    }
}