using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Tiles.Pyramid.Broken {
    public class PyramidStatueBroken1 : ModItem {
        public override string Texture => ModContent.GetInstance<PyramidStatue>().Texture;

        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<PyramidStatueBrokenTile>(), 0);
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
        }
    }

    public class PyramidStatueBroken2 : PyramidStatueBroken1 {
        public override void SetDefaults() {
            base.SetDefaults();
            Item.placeStyle = 1;
        }
    }
}