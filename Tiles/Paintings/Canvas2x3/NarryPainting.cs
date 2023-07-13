using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas2x3 {
    public class NarryPainting : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings2x3>(), WallPaintings2x3.NarryPainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}