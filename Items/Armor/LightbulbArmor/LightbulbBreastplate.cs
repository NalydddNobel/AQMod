using AQMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Armor.LightbulbArmor
{
    [AutoloadEquip(EquipType.Body)]
    public class LightbulbBreastplate : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.defense = 3;
            item.rare = ItemRarityID.Blue;
            item.value = Item.sellPrice(silver: 50);
        }

        public override bool DrawBody()
        {
            return true;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddIngredient(ItemID.CopperBar, 12);
            r.AddRecipeGroup("IronBar", 5);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
            r = new ModRecipe(mod);
            r.AddIngredient(ItemID.TinBar, 12);
            r.AddRecipeGroup("IronBar", 5);
            r.AddIngredient(ModContent.ItemType<Lightbulb>(), 3);
            r.AddTile(TileID.Anvils);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}