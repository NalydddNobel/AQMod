﻿using Aequus.Common.Items.SentryChip;
using Aequus.Content.Items.SentryChip;
using Aequus.Items.Materials.Glimmer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Equipment.Accessories.Combat {
    public class HyperCrystal : ModItem {
        public override void SetStaticDefaults() {
            SentryAccessoriesDatabase.Register<ApplyEquipFunctionalInteraction>(Type);
        }

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(gold: 1);
            Item.hasVanityEffects = true;
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(255, 255, 255, 200);
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            var aequus = player.Aequus();
            aequus.accHyperCrystal = Item;
            if (aequus.hyperCrystalCooldownMax > 0) {
                aequus.hyperCrystalCooldownMax = Math.Max(aequus.hyperCrystalCooldownMax / 2, 1);
            }
            else {
                aequus.hyperCrystalCooldownMax = 60;
            }
        }

        public void UpdateItemDye(Player player, bool isNotInVanitySlot, bool isSetToHidden, Item armorItem, Item dyeItem) {
            player.Aequus().cHyperCrystal = dyeItem.dye;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient<StariteMaterial>(20)
                .AddIngredient(ItemID.FallenStar, 8)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}