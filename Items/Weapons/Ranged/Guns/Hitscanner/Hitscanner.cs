using Aequus;
using Aequus.Common.Items;
using Aequus.Common.Particles;
using Aequus.Common.Recipes;
using Aequus.Content.World;
using Aequus.Items.Weapons.Magic.Misc.Healer;
using Aequus.Items.Weapons.Ranged.Guns.Hitscanner;
using Aequus.Particles;
using Aequus.Projectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Ranged.Guns.Hitscanner {
    public class Hitscanner : ModItem, ItemHooks.IOnSpawnProjectile {
        public override void SetStaticDefaults() {
            Item.ResearchUnlockCount = 1;
            HardmodeChestBoost.HardmodeJungleChestLoot.Add(Type);
        }

        public override void SetDefaults() {
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 30;
            Item.height = 30;
            Item.noMelee = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.LightRed;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 16f;
            Item.ArmorPenetration = 5;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = AequusSounds.doomShotgun;
            Item.value = Item.sellPrice(gold: 4);
            Item.autoReuse = true;
            Item.knockBack = 1f;
            Item.useTime = 60;
            Item.useAnimation = 60;
        }

        public override Vector2? HoldoutOffset() {
            return new Vector2(2f, 2f);
        }

        public int GetBulletAmount(int projectileID) {
            return projectileID == ProjectileID.ChlorophyteBullet ? 4 : 9;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            int amount = GetBulletAmount(type);
            for (int i = 0; i < amount; i++) {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)), type, damage, knockback, player.whoAmI);
                if (type == ProjectileID.ChlorophyteBullet) {
                    i++;
                }
            }
            return true;
        }

        public void InitalizeProjectile(Projectile projectile, AequusProjectile aequusProjectile) {
            projectile.extraUpdates++;
            if (projectile.type == ProjectileID.ChlorophyteBullet) {
                projectile.extraUpdates *= 10;
                projectile.damage *= 2;
            }
            else {
                projectile.extraUpdates *= 50;
            }
        }

        public override void AddRecipes() {
            AequusRecipes.AddShimmerCraft(Type, ModContent.ItemType<SavingGrace>());
        }

        public static Color GetBulletColor(Projectile projectile) {
            if (projectile.type == ProjectileID.Bullet) {
                return Color.OrangeRed with { A = 120 };
            }
            if (projectile.type == ProjectileID.CrystalShard) {
                return Color.BlueViolet with { A = 0 } * 0.33f;
            }

            return Raygun.Raygun.GetColor(projectile) with { A = 80 };
        }

        public static void EditParticleParams(Projectile projectile, ref Vector2 spawnLoc, ref Vector2 v, ref float length) {
            if (projectile.type == ProjectileID.ChlorophyteBullet) {
                length /= 2f;
                v *= 0.02f;
            }
        }

        public static void PostSpawnParticle(Projectile projectile, DashBlurParticle particle) {
            if (projectile.type == ProjectileID.ChlorophyteBullet) {
                return;
            }
            particle.animation = Main.rand.Next(-12, 3);
        }
    }
}

namespace Aequus.Projectiles {
    public partial class AequusProjectile {
        private void Hitscanner_ProjectileSpecificAttributes(Projectile projectile) {
            if (projectile.type == ProjectileID.ChlorophyteBullet) {
                projectile.alpha = 255;
            }
        }

        private void Hitscanner_ParticleSpecificParams(Projectile projectile, ref Vector2 spawnLoc, ref Vector2 v, ref float length) {
            if (projectile.type == ProjectileID.ChlorophyteBullet) {
                length /= 2f;
                v *= 0.02f;
            }
        }

        private void Hitscanner_ParticleSpecificAttributes(Projectile projectile, DashBlurParticle particle) {
            if (projectile.type == ProjectileID.ChlorophyteBullet) {
                return;
            }
            particle.animation = Main.rand.Next(-8, -5);
        }

        private void Effects_Hitscanner(Projectile projectile, bool onScreen) {
            if (sourceItemUsed != ModContent.ItemType<Hitscanner>()) {
                return;
            }

            Hitscanner_ProjectileSpecificAttributes(projectile);

            if (!onScreen || projectile.oldVelocity == Vector2.Zero
                || !Main.rand.NextBool(Math.Max(projectile.MaxUpdates / 100, 1)) || timeAlive < 3) {
                return;
            }

            var v = projectile.velocity;
            var spawnLoc = projectile.Center + v;
            float length = v.Length();
            var clr = Hitscanner.GetBulletColor(projectile);
            Hitscanner_ParticleSpecificParams(projectile, ref spawnLoc, ref v, ref length);
            var particle = ParticleSystem.New<DashBlurParticle>(ParticleLayer.BehindAllNPCs).Setup(
                spawnLoc,
                v * Main.rand.NextFloat(4f, 7f),
                clr.SaturationMultiply(Main.rand.NextFloat(0.5f, 0.8f)) with { A = clr.A } * 0.4f,
                scale: length / Main.rand.NextFloat(7f, 14f),
                rotation: v.ToRotation() + MathHelper.PiOver2
            );
            Hitscanner_ParticleSpecificAttributes(projectile, particle);
        }
    }
}