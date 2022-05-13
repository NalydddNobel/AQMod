using Aequus.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Banners
{
    public class CinderaBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            this.SetResearch(1);
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AequusBanners>(), AequusBanners.CinderaBanner);
            Item.rare = ItemDefaults.RarityBanner;
            Item.value = Item.sellPrice(silver: 2);
        }
    }
}