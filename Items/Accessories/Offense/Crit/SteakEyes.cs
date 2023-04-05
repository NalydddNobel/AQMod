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
        /// <summary>
        /// Default Value: 0.15
        /// <para>This is only added on the first stack of the accessory.</para>
        /// </summary>
        public static float NotStackableCritDamage = 0.15f;
        /// <summary>
        /// Default Value: 0.1
        /// </summary>
        public static float StackableCritDamage = 0.1f;

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
            aequus.highSteaksDamage = Math.Max(aequus.highSteaksDamage, NotStackableCritDamage) + StackableCritDamage;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HighSteaks>()
                .AddIngredient<PossessedShard>(5)
                .AddRecipeGroup(AequusRecipes.AnyEctoplasm, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}