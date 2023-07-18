using Aequus.Common.Items.SentryChip;
using Aequus.Content.Items.SentryChip;
using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Misc.ItemReach {
    public class HaltingMachine : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            SentryAccessoriesDatabase.Register<ApplyEquipFunctionalInteraction>(Type);
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.width = 16;
            Item.height = 16;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.hasVanityEffects = true;
            Item.value = Item.buyPrice(gold: 5);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().antiGravityItemRadius += 300f;
        }
    }
}