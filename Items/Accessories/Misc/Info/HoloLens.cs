using Aequus.Common.Recipes;
using Aequus.Content.Town.PhysicistNPC.Analysis;
using Aequus.Items.Accessories.CrownOfBlood;
using Aequus.Items.Tools;
using Aequus.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Misc.Info {
    public class HoloLens : ModItem {
        public override void SetStaticDefaults() {
            CrownOfBloodItem.NoBoost.Add(Type);
            AnalysisSystem.IgnoreItem.Add(Type);
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.LifeformAnalyzer);
            Item.rare = ItemRarityID.Orange;
        }

        public override void UpdateInfoAccessory(Player player) {
            if (Main.myPlayer == player.whoAmI)
                ChestLensInterface.Enabled = true;
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<PhaseMirror>());
        }
    }
}