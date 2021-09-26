using AQMod.Common.DeveloperTools;
using AQMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable.Banners
{
    public class HyperStariteBanner : ModItem
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
            item.placeStyle = AQBanners.HyperStarite;
        }

        public override void AddRecipes()
        {
            if (ModHelpers.Fargowiltas.Active)
            {
                var r = new ModRecipe(mod);
                r.AddIngredient(item.type);
                r.AddTile(TileID.Solidifier);
                r.SetResult(ItemID.Nazar);
                r.AddRecipe();
            }
        }
    }
}