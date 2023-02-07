using Aequus.Tiles.Furniture.HardmodeChests;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.HardmodeChests
{
    public class HardMarbleChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HardMarbleChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }
    }
}