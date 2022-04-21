﻿using Aequus.Items.Recipes;
using Aequus.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged
{
    public class SnowflakeCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.autoReuse = true;
            Item.damage = 36;
            Item.rare = ItemRarities.GaleStreams;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileID.SnowBallFriendly;
            Item.shootSpeed = 8f;
            Item.value = ItemPrices.GaleStreamsValue;
            Item.useAmmo = AmmoID.Snowball;
            Item.knockBack = 5.6f;
            Item.UseSound = SoundID.Item11;
            Item.noMelee = true;
        }

        public override bool CanConsumeAmmo(Player player)
        {
            return Main.rand.NextFloat() < 0.66f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(4f, 0f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            type = ModContent.ProjectileType<SnowflakeCannonBullet>();
            velocity = velocity.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f));
        }

        public override void AddRecipes()
        {
            ConsistentRecipes.SpaceSquidDrop(this, ItemID.SnowballCannon);
        }
    }
}