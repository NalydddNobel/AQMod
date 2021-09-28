using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.TagItems.Starbyte
{
    public class Molite : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 10);
            item.rare = ItemRarityID.Green;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(item.type);
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedShrimp, 5);
            r.AddRecipe();
        }
    }
}