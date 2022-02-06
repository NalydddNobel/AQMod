using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Materials
{
    public class Fleshscale : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.sellPrice(silver: 20);
            item.rare = ItemRarityID.Blue;
            item.maxStack = 999;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(item.type);
            r.AddTile(TileID.Anvils);
            r.SetResult(ItemID.TissueSample, 5);
            r.AddRecipe();
        }
    }
}