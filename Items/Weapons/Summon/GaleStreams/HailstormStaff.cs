﻿using Aequus.Buffs.Minion;
using Aequus.Common.Items;
using Aequus.Items.Materials.GaleStreams;
using Aequus.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.GaleStreams {
    public class HailstormStaff : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
        }

        public override void SetDefaults() {
            Item.damage = 32;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 3f;
            Item.value = ItemDefaults.ValueGaleStreams;
            Item.rare = ItemDefaults.RarityGaleStreams;
            Item.UseSound = SoundID.Item46;
            Item.buffType = ModContent.BuffType<SnowflakeBuff>();
            Item.shoot = ModContent.ProjectileType<SnowflakeMinion>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            player.AddBuff(Item.buffType, 2);
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback);
            return false;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.CobaltBar, 12)
                .AddIngredient<FrozenTear>(17)
                .AddTile(TileID.Anvils)
                .Register()
                .Clone().ReplaceItem(ItemID.CobaltBar, ItemID.PalladiumBar).Register();
        }
    }
}