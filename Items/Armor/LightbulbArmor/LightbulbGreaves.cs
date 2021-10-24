using AQMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.LightbulbArmor
{
    [AutoloadEquip(EquipType.Legs)]
    public class LightbulbGreaves : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 1;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 30);
        }

        public override void UpdateEquip(Player player)
        {
            player.maxMinions++;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.CopperBar, 8);
            r.AddRecipeGroup("IronBar", 3);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 2);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.TinBar, 8);
            r.AddRecipeGroup("IronBar", 3);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 2);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}