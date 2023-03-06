using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Furniture.Interactable
{
    public class CrabClock : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallClocks>(), WallClocks.CrabClock);
            Item.value = Item.buyPrice(gold: 1);
        }
    }
}