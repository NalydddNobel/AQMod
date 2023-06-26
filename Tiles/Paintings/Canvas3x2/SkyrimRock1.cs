using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas3x2 {
    public class SkyrimRock1 : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x2>(), WallPaintings3x2.Fus);
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(gold: 2);
        }
    }
}