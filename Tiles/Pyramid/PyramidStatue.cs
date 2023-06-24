using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Pyramid {
    public class PyramidStatue : ModItem {
        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<PyramidStatueTile>());
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Red;
        }
    }
}