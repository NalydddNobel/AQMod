using Aequus.Tiles.Crab;
using Terraria.ID;

namespace Aequus.Items.Placeable.Crab
{
    public class SedimentaryRock : BlockItemBase<SedimentaryRockTile>
    {
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SedimentaryRockWall>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}