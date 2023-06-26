using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas6x4 {
    public class BreadRoachPainting : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings6x4>(), WallPaintings6x4.BreadRoachPainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}