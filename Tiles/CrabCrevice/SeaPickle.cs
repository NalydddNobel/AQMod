using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.CrabCrevice {
    [LegacyName("SeaPickle")]
    public class SeaPickleItem : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<SeaPickleTile>());
            Item.value = Item.sellPrice(copper: 1);
        }
    }
}