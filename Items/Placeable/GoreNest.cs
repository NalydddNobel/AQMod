using Aequus.Tiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable
{
    public class GoreNest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GoreNestTile>());
            Item.value = Item.buyPrice(gold: 1, silver: 50);
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}