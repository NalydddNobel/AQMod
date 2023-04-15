using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Paintings.Items {
    public class SkyrimRock2 : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x2>(), WallPaintings3x2.Ro);
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}