using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Paintings
{
    public class YinPainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings2x2>(), WallPaintings2x2.YinPainting);
            Item.maxStack = 99;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}