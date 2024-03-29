﻿using Aequus.Common.Items;
using Aequus.Items.Materials.GaleStreams;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Swords.GaleStreams.Slice {
    public class Slice : ModItem {
        public override void SetDefaults() {
            Item.DefaultToAequusSword<SliceProj>(30);
            Item.SetWeaponValues(60, 2.5f, 6);
            Item.width = 20;
            Item.height = 20;
            Item.autoReuse = true;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.ValueGaleStreams;
        }

        public override bool? UseItem(Player player) {
            Item.FixSwing(player);
            return null;
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor.MaxRGBA(120);
        }

        public override bool MeleePrefix() {
            return true;
        }

        public override bool AltFunctionUse(Player player) {
            return false;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.CobaltBar, 14)
                .AddIngredient<FrozenTear>(20)
                .AddTile(TileID.Anvils)
                .Register()
                .Clone().ReplaceItem(ItemID.CobaltBar, ItemID.PalladiumBar).Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            return true;
        }
    }
}