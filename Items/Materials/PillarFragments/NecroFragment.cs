using Aequus.Common.Items;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Materials.PillarFragments {
    public class NecroFragment : ModItem {
#if !DEBUG
        public override bool IsLoadingEnabled(Mod mod) {
            return false;
        }
#endif

        public override void SetStaticDefaults() {
            ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.SolarFragment;
            ItemID.Sets.ItemIconPulse[Type] = true;
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.FragmentSolar);
            Item.Aequus().itemGravityCheck = 255;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.FragmentSolar)
                .AddIngredient(ItemID.FragmentVortex)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}