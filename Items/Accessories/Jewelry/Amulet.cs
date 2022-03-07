using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Accessories.Jewelry
{
    public class Amulet : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.defense = 2;
            item.rare = ItemRarityID.Green;
            item.value = Item.sellPrice(silver: 20);
            item.accessory = true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.Shackle);
            r.AddIngredient(ItemID.Sapphire, 4);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}