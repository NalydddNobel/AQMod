using Aequus.Common.Recipes;
using Aequus.Items.Materials;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories.Offense.Crit
{
    public class SteakEyes : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(24, 24);
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 1, silver: 50);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var aequus = player.Aequus();
            aequus.highSteaksDamage = Math.Max(aequus.highSteaksDamage, 0.25f) + 0.25f;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HighSteaks>()
                .AddIngredient<BloodyTearstone>(12)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}