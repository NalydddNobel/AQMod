﻿using Aequus.Common.Items;
using Aequus.Items.Materials.GaleStreams;
using Aequus.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Magic.GaleStreams.SurgeRod {
    [AutoloadGlowMask]
    public class SurgeRod : ModItem {
        public override void SetDefaults() {
            Item.width = 30;
            Item.height = 30;
            Item.DefaultToMagicWeapon(ModContent.ProjectileType<SurgeRodProj>(), 20, 30f, hasAutoReuse: false);
            Item.SetWeaponValues(36, 0f, 0);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.mana = 10;
            Item.UseSound = SoundID.Item66;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.value = ItemDefaults.ValueGaleStreams;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            Main.projectile[p].timeLeft = Math.Max((int)((Main.MouseWorld - position).Length() / velocity.Length()), 4);
            return false;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.CobaltBar, 16)
                .AddIngredient<Fluorescence>(18)
                .AddTile(TileID.Anvils)
                .Register()
                .Clone().ReplaceItem(ItemID.CobaltBar, ItemID.PalladiumBar).Register();
        }
    }
}