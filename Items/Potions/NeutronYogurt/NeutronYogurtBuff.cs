using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.NeutronYogurt; 

public class NeutronYogurtBuff : ModBuff {
    public override void Update(NPC npc, ref int buffIndex) {
        if (npc.velocity.Y > 0f) {
            npc.GravityMultiplier *= 2f;
        }
    }

    public override void Update(Player player, ref int buffIndex) {
        if (player.velocity.Y > 0f) {
            player.gravity *= 2f;
        }
    }
}