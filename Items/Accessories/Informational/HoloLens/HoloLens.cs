using Aequus.Common.Items.EquipmentBooster;
using Aequus.Common.Recipes;
using Aequus.Items.Tools.MagicMirrors.PhaseMirror;
using Aequus.NPCs.Town.PhysicistNPC.Analysis;

namespace Aequus.Items.Accessories.Informational.HoloLens;

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
        if (Main.myPlayer == player.whoAmI) {
            ChestLensInterface.IsEnabled = true;
        }
    }

    public override void AddRecipes() {
        AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<PhaseMirror>());
    }
}