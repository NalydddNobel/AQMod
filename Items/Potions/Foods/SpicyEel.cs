using AQMod.Buffs;
using AQMod.Items.Recipes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Potions.Foods
{
    public class SpicyEel : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 10;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.useAnimation = 15;
            item.useTime = 15;
            item.useTurn = true;
            item.UseSound = SoundID.Item2;
            item.maxStack = 30;
            item.consumable = true;
            item.rare = ItemRarityID.Green;
            item.value = Item.buyPrice(silver: 50);
            item.buffType = ModContent.BuffType<SpeedBoostFood>();
            item.buffTime = 25200;
        }

        public override void AddRecipes()
        {
            var r = new ModRecipe(mod);
            r.AddRecipeGroup(AQRecipeGroups.AnyEel);
            r.AddRecipeGroup(AQRecipeGroups.AnyNobleMushroom);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}