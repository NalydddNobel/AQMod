using AQMod.Assets;
using AQMod.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Weapons.Melee.Flails
{
    public class JerryClawFlail : ModItem
    {
        public override void SetDefaults()
        {
            item.damage = 32;
            item.melee = true;
            item.useTime = 30;
            item.useAnimation = 30;
            item.width = 30;
            item.height = 30;
            item.noMelee = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Blue;
            item.shoot = ModContent.ProjectileType<JerryClawFlailProjectile>();
            item.shootSpeed = 13.5f;
            item.UseSound = SoundID.Item1;
            item.value = AQItem.CrabsonWeaponValue;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.knockBack = 6f;
        }
    }

    public class JerryClawFlailProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 28;
            projectile.height = 28;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.melee = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active || player.dead)
            {
                projectile.Kill();
                return;
            }
            player.itemAnimation = 10;
            player.itemTime = 10;
            int direction = projectile.Center.X > player.Center.X ? 1 : -1;
            player.ChangeDir(direction);
            projectile.direction = direction;
            projectile.spriteDirection = -direction;
            Vector2 difference = player.MountedCenter - projectile.Center;
            float length = difference.Length();
            if (projectile.ai[0] == 0f)
            {
                float maxLength = 180f;
                projectile.tileCollide = true;
                if (length > maxLength)
                {
                    projectile.ai[0] = 1f;
                    projectile.netUpdate = true;
                }
                else if (!player.channel)
                {
                    if (projectile.velocity.Y < 0f)
                        projectile.velocity.Y *= 0.9f;
                    projectile.velocity.Y += 1f;
                    projectile.velocity.X *= 0.9f;
                }
            }
            else if (projectile.ai[0] == 1f)
            {
                float elasticFactorA = 18f / player.meleeSpeed;
                float elasticFactorB = 1f / player.meleeSpeed;
                float maxStretchLength = 300f;
                if (projectile.ai[1] == 1f)
                    projectile.tileCollide = false;
                if (!player.channel || length > maxStretchLength || !projectile.tileCollide)
                {
                    projectile.ai[1] = 1f;
                    if (projectile.tileCollide)
                        projectile.netUpdate = true;
                    projectile.tileCollide = false;
                    if (length < 20f)
                        projectile.Kill();
                }
                if (!projectile.tileCollide)
                    elasticFactorB *= 2f;
                int restingChainLength = 60;
                if (length > restingChainLength || !projectile.tileCollide)
                {
                    var elasticAcceleration = difference * elasticFactorA / length - projectile.velocity;
                    elasticAcceleration *= elasticFactorB / elasticAcceleration.Length();
                    projectile.velocity *= 0.98f;
                    projectile.velocity += elasticAcceleration;
                }
                else
                {
                    if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 6f)
                    {
                        projectile.velocity.X *= 0.96f;
                        projectile.velocity.Y += 0.2f;
                    }
                    if (player.velocity.X == 0f)
                        projectile.velocity.X *= 0.96f;
                }

            }
            projectile.rotation = difference.ToRotation() - MathHelper.PiOver2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            bool hitEffect = false;
            if (oldVelocity.X != projectile.velocity.X)
            {
                if (Math.Abs(oldVelocity.X) > 4f)
                    hitEffect = true;
                projectile.position.X += projectile.velocity.X;
                projectile.velocity.X = -oldVelocity.X * 0.2f;
            }
            if (oldVelocity.Y != projectile.velocity.Y)
            {
                if (Math.Abs(oldVelocity.Y) > 4f)
                    hitEffect = true;
                projectile.position.Y += projectile.velocity.Y;
                projectile.velocity.Y = -oldVelocity.Y * 0.2f;
            }
            projectile.ai[0] = 1f;
            if (hitEffect)
            {
                projectile.netUpdate = true;
                Collision.HitTiles(projectile.position, projectile.velocity, projectile.width, projectile.height);
                Main.PlaySound(SoundID.Dig, (int)projectile.position.X, (int)projectile.position.Y);
            }
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player player = Main.player[projectile.owner];
            Vector2 center = projectile.Center + new Vector2(0f, 10f).RotatedBy(projectile.rotation);
            Vector2 playerCenter = player.MountedCenter;
            var chain = TextureCache.JerryClawFlailProjectileChain.GetValue();
            int height = chain.Height - 2;
            var velo = Vector2.Normalize(center + new Vector2(0f, height * 4f) - playerCenter) * height;
            var position = playerCenter;
            Vector2 origin = new Vector2(chain.Width / 2f, chain.Height / 2f);
            for (int i = 0; i < 50; i++)
            {
                Main.spriteBatch.Draw(chain, position - Main.screenPosition, null, Lighting.GetColor((int)(position.X / 16), (int)(position.Y / 16f)), velo.ToRotation() + MathHelper.PiOver2, origin, 1f, SpriteEffects.None, 0f);
                velo = Vector2.Normalize(Vector2.Lerp(velo, center - position, 0.01f + MathHelper.Clamp(1f - Vector2.Distance(center, position) / 100f, 0f, 0.99f))) * height;
                position += velo;
                float gravity = MathHelper.Clamp(1f - Vector2.Distance(center, position) / 100f, 0f, 1f);
                velo.Y += gravity * 3f;
                velo.Normalize();
                velo *= height;
                if (Vector2.Distance(position, center) <= height)
                    break;
            }
            return true;
        }
    }
}