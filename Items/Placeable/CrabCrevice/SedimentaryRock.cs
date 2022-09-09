using Aequus.Tiles.CrabCrevice;
using Terraria.ID;

namespace Aequus.Items.Placeable.CrabCrevice
{
    public class SedimentaryRock : BlockItemBase<SedimentaryRockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SedimentaryRockWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register((r) => r.SortAfterFirstRecipesOf(ItemID.ObsidianBackEcho));
        }
    }
}