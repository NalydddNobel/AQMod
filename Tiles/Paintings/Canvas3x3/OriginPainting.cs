using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas3x3 {
    [LegacyName("Origin")]
    public class OriginPainting : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x3>(), WallPaintings3x3.OriginPainting);
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}