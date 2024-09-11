using Aequus.Common;

namespace Aequus.Content.Items.Materials.CompressedTrash;

[LegacyName("ItemScrap")]
[WorkInProgress]
public class CompressedTrash : ModItem {
    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 25;
    }

    public override void SetDefaults() {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ItemRarityID.White;
        Item.value = Item.sellPrice(copper: 50);
    }

    public override void AddRecipes() {
        /*
        CreateRecipe()
            .AddRecipeGroup(RecipeSystem.AnyTrash, 3)
            .AddTile<TrashCompactor>()
            .Register()
            .DisableDecraft();
        */
    }
}