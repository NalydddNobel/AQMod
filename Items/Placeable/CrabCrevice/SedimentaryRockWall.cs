using Aequus.Tiles.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.CrabCrevice
{
    public class SedimentaryRockWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlacableWall((ushort)ModContent.WallType<SedimentaryRockWallWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<SedimentaryRock>()
                .AddTile(TileID.WorkBenches)
                .TryRegisterAfter((ItemID.ObsidianBackEcho));
        }
    }
}