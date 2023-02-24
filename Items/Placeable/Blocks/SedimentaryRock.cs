using Aequus.Tiles.CrabCrevice;
using Terraria.ID;

namespace Aequus.Items.Placeable.Blocks
{
    public class SedimentaryRock : FancyBlockItemBase<SedimentaryRockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SedimentaryRockWall>(4)
                .AddTile(TileID.WorkBenches)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
        }
    }
}