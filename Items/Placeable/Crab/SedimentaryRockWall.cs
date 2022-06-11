using Aequus.Tiles.Crab;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Crab
{
    public class SedimentaryRockWall : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlacableWall((ushort)ModContent.WallType<SedimentaryRockWallWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<SedimentaryRock>()
                .AddTile(TileID.WorkBenches)
                .Register((r) => r.SortAfterFirstRecipesOf(ItemID.ObsidianBackEcho));
        }
    }
}