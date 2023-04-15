using Aequus.Items;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Tiles.Banners.Items {
    public class BreadOfCthulhuBanner : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<MonsterBanners>(), MonsterBanners.BreadofCthulhuBanner);
            Item.rare = ItemDefaults.RarityBanner;
            Item.value = Item.sellPrice(silver: 2);
        }
    }
}