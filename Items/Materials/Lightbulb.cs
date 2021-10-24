using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class Lightbulb : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 5);
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Glass);
            r.AddIngredient(ItemID.FallenStar);
            r.AddIngredient(ItemID.CopperBar);
            r.AddTile(TileID.Anvils);
            r.SetResult(this, 2);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Glass);
            r.AddIngredient(ItemID.FallenStar);
            r.AddIngredient(ItemID.TinBar);
            r.AddTile(TileID.Anvils);
            r.SetResult(this, 2);
            r.AddRecipe();
        }
    }
}