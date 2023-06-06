using Aequus.NPCs.Town.CarpenterNPC;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc.QuestFish {
    public class BrickFish : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 2;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.Batfish);
        }

        public override bool IsQuestFish() {
            return true;
        }

        public override bool IsAnglerQuestAvailable() {
            return NPC.AnyNPCs(ModContent.NPCType<Carpenter>());
        }

        public override void AnglerQuestChat(ref string description, ref string catchLocation) {
            description = TextHelper.GetTextValue("AnglerQuest.BrickFish.Description");
            catchLocation = TextHelper.GetTextValue("AnglerQuest.BrickFish.CatchLocation");
        }

        public static bool CheckVillagerBuildings(FishingAttempt attempt, Player player) {
            var comparePoint = new Vector2(attempt.X * 16f + 8f, attempt.Y * 16f + 8f);
            for (int i = 0; i < Main.maxNPCs; i++) {
                if (Main.npc[i].friendly && Main.npc[i].townNPC && !Main.npc[i].homeless) {
                    if (Vector2.Distance(Main.npc[i].Home().ToWorldCoordinates(), comparePoint) < 1000f) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}