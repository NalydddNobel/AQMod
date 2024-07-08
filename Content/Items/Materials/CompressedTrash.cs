using AequusRemake.Content.Tiles.CraftingStations.TrashCompactor;
using AequusRemake.Systems;

namespace AequusRemake.Content.Items.Materials;

[LegacyName("ItemScrap")]
public class CompressedTrash : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
        ItemSets.SortingPriorityMaterials[Type] = ItemSets.SortingPriorityMaterials[ItemID.Amber];
    }

    public override void SetDefaults() {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.White;
        Item.value = Item.sellPrice(copper: 50);
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddRecipeGroup(RecipeSystem.AnyTrash, 3)
            .AddTile<TrashCompactor>()
            .Register()
            .DisableDecraft();
    }
}