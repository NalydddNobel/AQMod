using Aequus.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Items.Placeable.Banners
{
    public class MagmabubbleBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<MonsterBanners>(), MonsterBanners.MagmabubbleBanner);
            Item.rare = ItemDefaults.RarityBanner;
            Item.value = Item.sellPrice(silver: 2);
        }
    }
}