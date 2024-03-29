﻿using Aequus.Buffs.Minion;
using Aequus.Common.Items;
using Aequus.Content.Events.DemonSiege;
using Aequus.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Summon.Demon {
    public class PiranhaPot : ModItem {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            DemonSiegeSystem.RegisterSacrifice(new SacrificeData(ModContent.ItemType<CorruptPot.CorruptPot>(), Type, EventTier.PreHardmode));
            ItemID.Sets.GamepadWholeScreenUseRange[Type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Type] = true;
        }

        public override void SetDefaults() {
            Item.damage = 22;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 10;
            Item.width = 26;
            Item.height = 28;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = ItemDefaults.ValueDemonSiege;
            Item.rare = ItemDefaults.RarityDemonSiege;
            Item.UseSound = SoundID.Item44;
            Item.shoot = ModContent.ProjectileType<PiranhaPlantMinion>();
            Item.buffType = ModContent.BuffType<PiranhaPlantBuff>();
            Item.autoReuse = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            player.AddBuff(Item.buffType, 2);
            player.SpawnMinionOnCursor(source, player.whoAmI, type, Item.damage, knockback, velocityOnSpawn: Main.rand.NextVector2Unit() * 5f);
            return false;
        }
    }
}