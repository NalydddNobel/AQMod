using Aequus.Core.Entities.Items;

namespace Aequus.Old.Content.Items.Materials.PillarFragments;

public class ArtFragment : ModItem {
    public override void SetStaticDefaults() {
        ItemSets.SortingPriorityMaterials[Type] = ItemSets.SortingPriorityMaterials[ItemID.FragmentSolar];
        ItemSets.ItemIconPulse[Type] = true;
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.FragmentSolar);
        Item.GetGlobalItem<GravityGlobalItem>().itemGravityCheck = 255;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override void AddRecipes() {
#if DEBUG
        CreateRecipe()
            .AddIngredient(ItemID.FragmentSolar)
            .AddIngredient(ItemID.FragmentVortex)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
#endif
    }
}