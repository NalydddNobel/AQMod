using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas2x2 {
    public class YangPainting : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings2x2>(), WallPaintings2x2.YangPainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}