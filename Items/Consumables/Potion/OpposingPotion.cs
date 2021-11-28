using AQMod.Buffs;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Consumables.Potion
{
    public class OpposingPotion : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 26;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item3;
            item.maxStack = 999;
            item.consumable = true;
            item.rare = ItemRarityID.LightRed;
            item.value = AQItem.Prices.PotionValue;
            item.buffTime = 36000;
            item.buffType = ModContent.BuffType<Opposing>();
        }

        public override void AddRecipes()
        {
            var recipe = new ModRecipe(mod);
            recipe.alchemy = true;
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.Daybloom);
            recipe.AddIngredient(ItemID.Deathweed);
            recipe.AddIngredient(ItemID.SoulofLight);
            recipe.AddIngredient(ItemID.SoulofNight);
            recipe.AddTile(TileID.Bottles);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}