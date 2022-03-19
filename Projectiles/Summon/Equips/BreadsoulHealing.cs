using AQMod.Assets;
using AQMod.Common.ID;
using AQMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Summon.Equips
{
    public class BreadsoulHealing : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 14;
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.penetrate = 2;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 100;
            projectile.timeLeft = 360;
            projectile.extraUpdates = 1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] >= 100)
            {
                return;
            }
            int target = -1;
            float closestDist = 1200f;
            var center = projectile.Center;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].minion && Main.projectile[i].minionSlots > 0f)
                {
                    var distance = Vector2.Distance(center, Main.projectile[i].Center);
                    if (Main.projectile[i].owner != projectile.owner) // Minions which are not the owner of this proj need to be x2 closer compared to the owner's minions in order to get a buff!
                    {
                        distance *= 2f;
                    }
                    if (distance < closestDist)
                    {
                        target = i;
                        closestDist = distance;
                    }
                }
            }
            if (target > -1)
            {
                projectile.ai[1]++;
                var difference = Main.projectile[target].Center - center;
                if (difference.Length() < 200f)
                {
                    projectile.tileCollide = false;
                }
                if (difference.Length() < 20f)
                {
                    OnHitAnything(projectile.velocity);
                    AQProjectile.ApplyDMGBuff(Main.projectile[target], 0.25f, 180, AQProjectile.DamageBuffEffects.MinionBuff, alwaysOverride: false);
                    return;
                }
                float amount = 0.002f;
                if (projectile.ai[1] > 15f)
                {
                    amount = 0.15f;
                }
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(difference) * 15f, amount);
            }
            else
            {
                projectile.tileCollide = true;
                if (projectile.ai[1] > MathHelper.TwoPi)
                {
                    projectile.ai[1] += 0.12f;
                    if (projectile.ai[1] > MathHelper.TwoPi * 2f)
                    {
                        projectile.ai[1] = MathHelper.TwoPi;
                    }
                }
                else
                {
                    projectile.ai[1] += 0.3f;
                }
                projectile.velocity = projectile.velocity.RotatedBy(Math.Sin(projectile.ai[1]) * 0.01f);
            }
            projectile.rotation = projectile.velocity.ToRotation();
            if (Main.rand.NextBool(5))
            {
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(200, 200, 255, 10));
                d.velocity = projectile.velocity * 0.2f;
            }
        }

        private void OnHitAnything(Vector2 velocity)
        {
            velocity = Vector2.Normalize(velocity);
            for (int i = 0; i < 18; i++)
            {
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(200, 200, 255, 10));
                d.velocity = -velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(2f, 5f);
            }
            for (int i = 0; i < 4; i++)
            {
                var d = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, ModContent.DustType<MonoSparkleDust>(), 0f, 0f, 0, new Color(200, 200, 255, 10));
                d.velocity = -velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(3f, 6f);
            }
            projectile.damage = 0;
            projectile.tileCollide = false;
            projectile.ai[0] = 100f;
            projectile.velocity = Vector2.Zero;
            if (projectile.timeLeft > 15)
            {
                projectile.timeLeft = 15;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitAnything(projectile.velocity);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            OnHitAnything(oldVelocity);
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            lightColor = Color.White;
            var texture = Main.projectileTexture[projectile.type];
            var frame = texture.Frame(verticalFrames: Main.projFrames[projectile.type], frameY: projectile.frame);
            var origin = frame.Size() / 2f;
            var offset = new Vector2(projectile.width / 2f, projectile.height / 2f + projectile.gfxOffY) - Main.screenPosition;

            int start = 0;
            if (projectile.timeLeft < 15)
            {
                start = 15 - projectile.timeLeft;
            }
            int trailLength = ProjectileID.Sets.TrailCacheLength[projectile.type];
            if (!AQMod.LowQ)
            {
                var effectTexture = LegacyTextureCache.Lights[LightTex.Spotlight66x66];
                var effectFrame = effectTexture.Frame();
                var effectOrigin = effectFrame.Size() / 2f;
                for (int i = start; i < trailLength; i++)
                {
                    float progress = 1f / trailLength * i;
                    spriteBatch.Draw(effectTexture, projectile.oldPos[i] + offset, effectFrame, new Color(30, 35, 60, 255) * (1f - progress) * 0.65f, 0f, effectOrigin, projectile.scale * (1f - progress) * 0.6f, SpriteEffects.None, 0f);
                }
                if (projectile.timeLeft >= 15)
                    spriteBatch.Draw(effectTexture, projectile.position + offset, effectFrame, new Color(50, 60, 100, 255), 0f, effectOrigin, projectile.scale, SpriteEffects.None, 0f);
            }
            for (int i = start; i < trailLength; i++)
            {
                float progress = 1f / trailLength * i;
                spriteBatch.Draw(texture, projectile.oldPos[i] + offset, frame, new Color(255, 255, 255, 0) * (1f - progress), projectile.rotation, origin, projectile.scale * (1f - progress), SpriteEffects.None, 0f);
            }
            if (projectile.timeLeft >= 15)
                spriteBatch.Draw(texture, projectile.position + offset, frame, lightColor, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}