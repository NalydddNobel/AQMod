using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Misc
{
    public class Nessie : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 40);
            item.rare = ItemRarityID.Blue;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(item.type);
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedFish);
            r.AddRecipe();
        }
    }
}