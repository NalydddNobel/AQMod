using Aequus.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.CrabCrevice
{
    public class SeaPickle : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GoreNestTile>());
            Item.value = Item.buyPrice(silver: 2);
        }
    }
}