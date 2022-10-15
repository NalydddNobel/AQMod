using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture
{
    public class CosmicMonolith : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.BloodMoonMonolith);
            Item.createTile = ModContent.TileType<CosmicMonolithTile>();
            Item.placeStyle = 0;
        }
    }
}