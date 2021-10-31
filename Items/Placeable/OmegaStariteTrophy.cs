using AQMod.Common;
using AQMod.Items.Weapons.Magic;
using AQMod.Items.Weapons.Ranged;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable
{
    public class OmegaStariteTrophy : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = 50000;
            item.rare = ItemRarityID.Blue;
            item.createTile = ModContent.TileType<Tiles.Trophies>();
            item.placeStyle = 0;
        }

        public override void AddRecipes()
        {
            if (FargosQOLStuff.FargowiltasActive)
            {
                var r = new ModRecipe(mod);
                r.AddIngredient(item.type);
                r.AddTile(TileID.Solidifier);
                r.SetResult(ModContent.ItemType<Raygun>());
                r.AddRecipe();
                r = new ModRecipe(mod);
                r.AddIngredient(item.type);
                r.AddTile(TileID.Solidifier);
                r.SetResult(ModContent.ItemType<MagicWand>());
                r.AddRecipe();
            }
        }
    }
}