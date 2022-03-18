using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable
{
    public class PetrifiedWoodPlatform : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<Tiles.PetrifiedFurn.PetrifiedWoodPlatform>();
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<PetrifiedWood>());
            r.SetResult(this, 2);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(this, 2);
            r.SetResult(ModContent.ItemType<PetrifiedWood>());
            r.AddRecipe();
        }
    }
}