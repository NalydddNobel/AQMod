using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class MagicalBoomerang : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.extraUpdates = 2;
            projectile.manualDirectionChange = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == 0)
            {
                projectile.direction = projectile.velocity.X < 0f ? -1 : 1;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] > 60f)
                projectile.tileCollide = false;
            if (projectile.ai[0] > 46f)
            {
                float speed = Math.Max((Main.player[projectile.owner].Center - projectile.Center).Length() / 60f, 10f) + projectile.ai[0] * 0.0003f;
                projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(Main.player[projectile.owner].Center - projectile.Center) * speed, Math.Max(1f - (Main.player[projectile.owner].Center - projectile.Center).Length() / 40f, 0.04f));
                if ((projectile.Center - Main.player[projectile.owner].Center).Length() < 20f)
                {
                    projectile.Kill();
                }
            }
            projectile.rotation += 0.125f;
            var hitbox = projectile.getRect();
            for (int i = 0; i < Main.maxItems; i++)
            {
                if (Main.item[i] != null && Main.item[i].active && Main.item[i].getRect().Intersects(hitbox))
                {
                    Main.item[i].Center = projectile.Center;
                    Main.itemLockoutTime[i] = 2;
                    if (projectile.ai[0] < 400f)
                        projectile.ai[0] = 400f;
                    break;
                }
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 4;
            height = 4;
            fallThrough = true;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.velocity = oldVelocity;
            if (projectile.ai[0] < 400f)
                projectile.ai[0] = 400f;
            projectile.velocity = -oldVelocity;
            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250, 250);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            var texture = Main.projectileTexture[projectile.type];
            Rectangle frame = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 offset = new Vector2(projectile.width / 2, projectile.height / 2);
            var effects = projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var color = new Color(40, 80, 150, 40);
            var origin = frame.Size() / 2f;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type]; i++)
            {
                float progress = 1f / ProjectileID.Sets.TrailCacheLength[projectile.type] * i;
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + offset - Main.screenPosition, frame, color * (1f - progress), projectile.rotation, origin, Math.Max(projectile.scale * (1f - progress), 0.1f), effects, 0f);
            }

            Main.spriteBatch.Draw(texture, projectile.position + offset - Main.screenPosition, null, new Color(255, 255, 255, 255), projectile.rotation, origin, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}