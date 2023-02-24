using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.NPCs.ExporterNPC.Shop
{
    public class CrabClock : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallClocks>(), WallClocks.CrabClock);
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}