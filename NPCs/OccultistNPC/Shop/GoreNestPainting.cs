using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.NPCs.OccultistNPC.Shop
{
    public class GoreNestPainting : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings>(), WallPaintings.GoreNestPainting);
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}