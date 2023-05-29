using Aequus.Tiles.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    public class UltraStariteBanner : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 0;
        }

        public override void SetDefaults() {
            Item.DefaultToPlaceableTile(ModContent.TileType<MonsterBanners>(), MonsterBanners.UltraStariteBanner);
            Item.rare = ItemRarityID.Gray;
            Item.value = Item.sellPrice(silver: 2);
        }
    }
}