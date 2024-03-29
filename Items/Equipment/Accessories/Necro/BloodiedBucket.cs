﻿using Aequus.Common;
using Aequus.Common.Items;
using Aequus.Common.Recipes;
using Aequus.Items.Weapons.Ranged.Bows.CrusadersCrossbow;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Necro {
    [WorkInProgress]
    public class BloodiedBucket : ModItem {
        /// <summary>
        /// Default Value: 3600 (1 minute)
        /// </summary>
        public static int GhostLifespan = 3600;

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = ItemDefaults.ValueBloodMoon * 3;
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Aequus().ghostLifespan += GhostLifespan;
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<CrusadersCrossbow>());
        }
    }
}