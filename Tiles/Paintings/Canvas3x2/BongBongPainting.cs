using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas3x2 {
    public class BongBongPainting : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x2>(), WallPaintings3x2.BongBongPainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}