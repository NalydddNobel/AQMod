using Aequus.Tiles.Moss;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Nature.Moss
{
    public class RadonMoss : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<RadonMossTile>());
        }
    }
}