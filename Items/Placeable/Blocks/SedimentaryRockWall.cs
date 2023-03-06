using Aequus.Tiles.CrabCrevice;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Blocks
{
    public class SedimentaryRockWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<SedimentaryRockWallWall>());
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
                .AddIngredient<SedimentaryRock>()
                .AddTile(TileID.WorkBenches)
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
        }
    }
}