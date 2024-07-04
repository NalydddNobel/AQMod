using AequusRemake.Content.Items.Materials;

namespace AequusRemake.Content.Tiles.PollutedOcean.Trees;

public class ScrapTree : ScrapPalmTree {
    public override void AddRecipes() {
        DropItem.CreateRecipe()
            .AddIngredient(ModContent.ItemType<CompressedTrash>(), 3)
            .Register();

        TreeTopItem.CreateRecipe()
            .AddIngredient(DropItem.Type, 15)
            .Register();
    }
}
