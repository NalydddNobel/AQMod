using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Paintings
{
    public class ExLydSpacePainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings>(), WallPaintings.ExLydSpacePainting);
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}