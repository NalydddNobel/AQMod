using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Fishing
{
    public class Blobfish : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(gold: 1);
            item.rare = ItemRarityID.Green;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(item.type);
            r.AddTile(TileID.CookingPots);
            r.SetResult(ItemID.CookedFish, 5);
            r.AddRecipe();
        }
    }
}