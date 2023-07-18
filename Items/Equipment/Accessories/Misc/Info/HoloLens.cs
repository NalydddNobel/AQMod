using Aequus.Common.Recipes;
using Aequus.Content.UI;
using Aequus.Common.Items.EquipmentBooster;
using Aequus.NPCs.Town.PhysicistNPC.Analysis;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Aequus.Items.Tools.MagicMirrors.PhaseMirror;

namespace Aequus.Items.Equipment.Accessories.Misc.Info {
    public class HoloLens : ModItem {
        public override void SetStaticDefaults() {
            EquipBoostDatabase.Instance.SetNoEffect(Type);
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