using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture
{
    public class FishSign : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<FishSignTile>());
            Item.value = Item.buyPrice(gold: 2, silver: 50);
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.White;
        }
    }
}