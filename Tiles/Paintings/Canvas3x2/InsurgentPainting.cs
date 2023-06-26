using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas3x2 {
    public class InsurgentPainting : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x2>(), WallPaintings3x2.InsurgentPainting);
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}