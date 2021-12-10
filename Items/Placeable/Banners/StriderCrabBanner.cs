using AQMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Banners
{
    public class StriderCrabBanner : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 24;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 2);
            item.createTile = ModContent.TileType<AQBanners>();
            item.placeStyle = AQBanners.StriderCrab;
        }
    }
}