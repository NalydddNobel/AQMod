using Aequus.Tiles.Furniture.Gravity;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Gravity
{
    public class GravityChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GravityChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }
    }
}