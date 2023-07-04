using Aequus.Content.Events.GlimmerEvent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.NPCs.Global {
    public class NPCDialogue : GlobalNPC {
        public override void GetChat(NPC npc, ref string chat) {
            if (Main.LocalPlayer.Aequus().ZoneGlimmer && Main.rand.NextBool(3) && TextHelper.TryGetValue($"Mods.Aequus.Chat.{TextHelper.NPCKeyName(npc.type)}.Glimmer", out var text)) {
                chat = text;
            }
            if (npc.type == NPCID.SkeletonMerchant && GlimmerBiomeManager.EventActive && Main.rand.NextBool()) {
                chat = TextHelper.GetTextValue("Chat.SkeletonMerchant.Glimmer");
            }
        }
    }
}