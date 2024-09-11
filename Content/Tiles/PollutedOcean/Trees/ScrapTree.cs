#if POLLUTED_OCEAN_TODO
using Aequus.Content.Items.Materials;

namespace Aequus.Content.Tiles.PollutedOcean.Trees;

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
#endif