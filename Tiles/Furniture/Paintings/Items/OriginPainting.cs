using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Furniture.Paintings.Items {
    [LegacyName("Origin")]
    public class OriginPainting : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings>(), WallPaintings.OriginPainting);
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}