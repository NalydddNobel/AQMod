using Aequus.Tiles.CrabCrevice;
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
            Item.DefaultToPlaceableTile(ModContent.TileType<SeaPickleTile>());
            Item.value = Item.buyPrice(silver: 2);
        }
    }
}