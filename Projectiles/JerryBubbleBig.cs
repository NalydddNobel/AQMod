using AQMod.Assets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class JerryBubbleBig : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.timeLeft = 300;
            projectile.aiStyle = -1;
            projectile.ignoreWater = true;
        }

        public void ImpactJerryBubbles()
        {
            var center = projectile.Center;
            float size = projectile.Size.Length();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].whoAmI != projectile.whoAmI && Main.projectile[i].type == projectile.type)
                {
                    var difference = Main.projectile[i].Center - center;
                    if (difference.Length() < size)
                    {
                        var normal = Vector2.Normalize(difference);
                        float speed = projectile.velocity.Length() * 0.8f;
                        projectile.velocity = Vector2.Lerp(projectile.velocity, -normal * speed, 0.6f);
                        Main.projectile[i].position += normal * Main.projectile[i].velocity.Length();
                        Main.projectile[i].velocity = Vector2.Lerp(Main.projectile[i].velocity, normal * speed / 6f, 0.4f);
                    }
                }
            }
        }

        public override void AI()
        {
            float speed = -3f;
            if (Collision.WetCollision(projectile.position, projectile.width, projectile.height))
                speed *= 3f;
            if (Main.expertMode)
                speed *= 2f;
            Player player = Main.player[Player.FindClosest(projectile.position, projectile.width, projectile.height)];
            if (player.dead || !player.active)
            {
                projectile.velocity.X *= 0.8f;
                if (projectile.velocity.Y > speed)
                    projectile.velocity.Y += speed / 60f;
            }
            else
            {
                var gotoPosition = player.MountedCenter + new Vector2(0f, -200f);
                var difference = projectile.Center - gotoPosition;
                if (difference.Length() > projectile.width)
                {
                    var gotoVelocity = Vector2.Normalize(difference);
                    gotoVelocity.X *= 0.8f;
                    gotoVelocity *= speed;
                    projectile.velocity.X = MathHelper.Lerp(projectile.velocity.X, gotoVelocity.X, 0.02f);
                    projectile.velocity.Y = MathHelper.Lerp(projectile.velocity.Y, gotoVelocity.Y, 0.0075f);
                }
            }
            ImpactJerryBubbles();
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (lightColor.R < 40)
                lightColor.R = 40;
            if (lightColor.G < 40)
                lightColor.G = 40;
            if (lightColor.B < 40)
                lightColor.B = 40;
            lightColor.A = (byte)(120 + (Math.Sin(projectile.timeLeft * 0.2f) + 1f) / 2f * 75f);
            return lightColor;
        }

        public override void Kill(int timeLeft)
        {
            var center = projectile.Center;
            float maxSpawnOffsetRadius = projectile.width / 2f;
            int projectileType = ModContent.ProjectileType<JerryBubble>();
            float speed = projectile.velocity.Length();
            bool wet = Collision.WetCollision(projectile.position, projectile.width, projectile.height);
            int hostileBubbleCount = 4;
            if (Main.expertMode)
                hostileBubbleCount = (int)(hostileBubbleCount * 1.5f);
            if (wet)
            {
                hostileBubbleCount *= 2;
                for (int i = 0; i < hostileBubbleCount * 2; i++)
                {
                    Dust.NewDust(center - new Vector2(-10f, -10f), 20, 20, 34);
                }
            }
            int damage = projectile.damage / 2;
            for (int i = 0; i < hostileBubbleCount; i++)
            {
                var spawnOffsetNormal = new Vector2(0f, 1f).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
                float spawnOffsetRadius = Main.rand.NextFloat(maxSpawnOffsetRadius);
                Projectile.NewProjectile(center + spawnOffsetNormal * spawnOffsetRadius, spawnOffsetNormal * speed, projectileType, damage, projectile.knockBack, projectile.owner, -180f);
            }
            for (int i = 0; i < 50; i++)
            {
                var spawnOffsetNormal = new Vector2(1f, 0f).RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi));
                float spawnOffsetRadius = Main.rand.NextFloat(maxSpawnOffsetRadius);
                int d = Dust.NewDust(center + spawnOffsetNormal * spawnOffsetRadius, 2, 2, 15);
                Main.dust[d].scale *= projectile.scale - 0.1f;
                Main.dust[d].noLight = true;
                Main.dust[d].noGravity = true;
                Main.dust[d].velocity = spawnOffsetNormal * 7f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = this.GetTexture();
            var drawPosition = projectile.Center - Main.screenPosition;
            lightColor = projectile.GetAlpha(lightColor);
            var size = lightColor.A / 255f + 0.1f;
            var orig = texture.Size() / 2f;
            Main.spriteBatch.Draw(texture, drawPosition, null, lightColor, projectile.rotation, orig, projectile.scale * size, SpriteEffects.None, 0f);
            return false;
        }
    }
}