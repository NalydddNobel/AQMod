using Aequus.Tiles.CraftingStation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.CraftingStation
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
            Item.value = Item.buyPrice(gold: 5);
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}