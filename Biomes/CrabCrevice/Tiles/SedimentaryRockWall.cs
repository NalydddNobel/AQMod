using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Biomes.CrabCrevice.Tiles
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
                .TryRegisterAfter(ItemID.ObsidianBackEcho);
        }
    }

    public class SedimentaryRockWallWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            DustType = 209;
            ItemDrop = ModContent.ItemType<SedimentaryRockWall>();
            AddMapEntry(new Color(70, 40, 20));
        }
    }
}