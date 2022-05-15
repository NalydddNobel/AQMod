using Aequus.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable
{
    public class GoreNest : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GoreNestTile>());
            Item.maxStack = 99;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}