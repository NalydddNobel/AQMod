using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials.Fish
{
    public class Combfish : ModItem
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
            r.AddIngredient(ItemID.BottledWater);
            r.AddIngredient(item.type);
            r.AddIngredient(ItemID.Moonglow);
            r.AddIngredient(ItemID.Shiverthorn);
            r.AddIngredient(ItemID.Waterleaf);
            r.AddTile(TileID.Bottles);
            r.SetResult(ItemID.LifeforcePotion);
            r.AddRecipe();
        }
    }
}