using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Potions.NeutronYogurt {
    public class NeutronYogurtBuff : ModBuff {
        public override void Update(NPC npc, ref int buffIndex) {
            npc.GravityMultiplier *= 1.35f;
        }

        public override void Update(Player player, ref int buffIndex) {
            player.gravity *= 1.35f;
        }
    }
}