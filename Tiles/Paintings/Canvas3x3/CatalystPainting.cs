using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Paintings.Canvas3x3 {
    public class CatalystPainting : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<WallPaintings3x3>(), WallPaintings3x3.RockFromAnAlternateUniversePainting);
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.Blue;
        }
    }
}