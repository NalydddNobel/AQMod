using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Banners
{
    public class WhiteSlimeBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MonsterBanners>(), MonsterBanners.WhiteSlimeBanner);
            Item.rare = ItemDefaults.RarityBanner;
            Item.value = Item.sellPrice(silver: 2);
        }
    }
}