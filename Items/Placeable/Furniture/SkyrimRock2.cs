using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture
{
    public class SkyrimRock2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x2>(), WallPaintings3x2.Ro);
            Item.maxStack = 99;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}