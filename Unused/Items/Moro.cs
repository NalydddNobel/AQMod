﻿using Aequus.Common;
using Aequus.Common.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Unused.Items {
    [UnusedContent]
    public class Moro : ModItem {
        public override void SetDefaults() {
            Item.DefaultToHoldUpItem();
            Item.width = 24;
            Item.height = 24;
            Item.consumable = true;
            Item.rare = ItemDefaults.RarityShimmerPermanentUpgrade;
            Item.UseSound = SoundID.Item92;
            Item.value = Item.sellPrice(gold: 2);
            Item.maxStack = Item.CommonMaxStack;
        }

        public override bool? UseItem(Player player) {
            return false;
        }

        public override void AddRecipes() {
            //CreateRecipe()
            //    .AddRecipeGroup(AequusRecipes.AnyFruit, 3)
            //    .AddIngredient<Fluorescence>(10)
            //    .AddIngredient<AtmosphericEnergy>()
            //    .AddTile(TileID.Anvils)
            //    .Register();
        }
    }
}