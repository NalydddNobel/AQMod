using Aequus.Tiles.Moss;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Moss
{
    public class RadonMoss : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RadonMossTile>());
        }
    }
}