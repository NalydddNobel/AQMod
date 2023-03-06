using Aequus.Tiles.Furniture.Oblivion;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Oblivion
{
    public class OblivionChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<OblivionChestTile>());
            Item.value = Item.sellPrice(silver: 10);
        }
    }
}