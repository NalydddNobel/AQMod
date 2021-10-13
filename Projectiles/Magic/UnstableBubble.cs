using AQMod.Assets;
using AQMod.Assets.Textures;
using AQMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class UnstableBubble : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.magic = true;
            projectile.penetrate = -1;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 400;
            projectile.scale = 0.6f;
        }

        public override void AI()
        {
            if (projectile.ai[0] == -1f)
            {
                projectile.velocity *= 0.98f;
                return;
            }
            if (projectile.ai[0] <= 0f)
            {
                float closestDistance = 400f;
                int closestProjectile = -1;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (i == projectile.whoAmI)
                        continue;
                    if (Main.projectile[i].active && Main.projectile[i].type == projectile.type && Main.projectile[i].owner == projectile.owner)
                    {
                        float dist = projectile.Distance(Main.projectile[i].Center);
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            closestProjectile = i;
                        }
                    }
                }
                if (closestProjectile == -1)
                {
                    return;
                }
                projectile.ai[0] = closestProjectile + 1;
            }
            if (projectile.alpha < byte.MaxValue)
            {
                projectile.alpha += 10;
                if (projectile.alpha > byte.MaxValue)
                    projectile.alpha = byte.MaxValue;
            }
            int nextBubble = (int)(projectile.ai[0] - 1);
            var differenceToConnection = Main.projectile[nextBubble].Center - projectile.Center;
            float distance = differenceToConnection.Length();
            if (distance > projectile.width * 2)
            {
                projectile.velocity = Vector2.Lerp(projectile.velocity, differenceToConnection, 0.008f);
            }
            else if (distance < projectile.width)
            {
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(-differenceToConnection) * 1f, 0.05f);
            }
            if (Main.rand.NextBool(4))
            {
                var type = ModContent.DustType<MonoDust>();
                var color = new Color(120, 120, 240, 0);
                if (Main.rand.NextBool(8))
                {
                    color = new Color(170, 140, 255, 20);
                }
                int d = Dust.NewDust(projectile.Center, 2, 2, type, 0f, 0f, 0, color);
                Main.dust[d].velocity = Vector2.Normalize(differenceToConnection) * (differenceToConnection.Length() / 9f);
                Main.dust[d].velocity += projectile.velocity;
                Main.dust[d].velocity *= Main.rand.NextFloat(0.9f, 1.1f);
            }
        }

        public override void Kill(int timeLeft)
        {
            if (projectile.alpha < 200)
                return;
            Main.PlaySound(SoundID.Item118, projectile.position);
            Main.PlaySound(SoundID.Item112, projectile.position);
            int count = 40;
            float rot = MathHelper.TwoPi / count;
            var center = projectile.Center + new Vector2(-1f, -1f);
            var type = ModContent.DustType<MonoDust>();
            var color = new Color(50, 15, 190, 0);
            float off = projectile.width / 2f;
            float dustSpeed = 4f;
            for (int i = 0; i < count; i++)
            {
                var normal = new Vector2(1f, 0f).RotatedBy(rot * i);
                int d = Dust.NewDust(center + normal * off, 2, 2, type, 0f, 0f, 0, color);
                Main.dust[d].velocity = normal * dustSpeed;
            }
            color *= 2f;
            dustSpeed *= 0.7f;
            for (int i = 0; i < count; i++)
            {
                var normal = new Vector2(1f, 0f).RotatedBy(rot * i);
                int d = Dust.NewDust(center + normal * off, 2, 2, type, 0f, 0f, 0, color);
                Main.dust[d].velocity = normal * dustSpeed;
            }
            color *= 2f;
            dustSpeed *= 0.7f;
            for (int i = 0; i < count; i++)
            {
                var normal = new Vector2(1f, 0f).RotatedBy(rot * i);
                int d = Dust.NewDust(center + normal * off, 2, 2, type, 0f, 0f, 0, color);
                Main.dust[d].velocity = normal * dustSpeed;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            var offset = new Vector2(projectile.width / 2f - Main.screenPosition.X, projectile.height / 2f - Main.screenPosition.Y);
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var origin = frame.Size() / 2f;
            var color = new Color(250, 250, 250, 128);
            color *= (projectile.alpha / 255f);
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color, projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, projectile.position + offset, frame, color * 0.4f, projectile.rotation, origin, projectile.scale + (float)Math.Sin(Main.GlobalTime * 10f) * 0.1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
