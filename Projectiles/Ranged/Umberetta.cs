using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Ranged
{
    public class Umberetta : ModProjectile
    {
        public List<Vector2> goBackPositions;

        public override void AI()
        {
            Main.player[projectile.owner].itemTime = 2;
            Main.player[projectile.owner].itemAnimation = 2;
            if ((int)projectile.ai[0] == -2)
            {
                if (goBackPositions.Count <= 0)
                {
                    projectile.Kill();
                }
                return;
            }
            projectile.ai[0]--;
            if (projectile.ai[0] <= 0f)
            {
                if (projectile.ai[0] == -1f)
                {
                    goBackPositions.Add(projectile.Center);
                }
                projectile.ai[0] = 25f;
                goBackPositions.Add(projectile.Center);
            }
            goBackPositions[0] = projectile.Center;
        }

        public override bool PreDrawExtras(SpriteBatch spriteBatch)
        {
            var mountedCenter = Main.player[projectile.owner].MountedCenter;
            var projectileChainPos = projectile.Center - Vector2.Normalize(projectile.velocity) * 8f;
            if (projectile.alpha == 0)
            {
                int num28 = -1;
                if (projectile.position.X + projectile.width / 2 < mountedCenter.X)
                {
                    num28 = 1;
                }
                if (Main.player[projectile.owner].direction == 1)
                {
                    Main.player[projectile.owner].itemRotation = (float)Math.Atan2(mountedCenter.Y - projectileChainPos.Y * num28, mountedCenter.X - projectileChainPos.X * num28);
                }
                else
                {
                    Main.player[projectile.owner].itemRotation = (float)Math.Atan2(mountedCenter.Y - projectileChainPos.Y * num28, mountedCenter.X - projectileChainPos.X * num28);
                }
            }

            for (int i = 0; i < goBackPositions.Count; i++)
            {
                if (i > 0)
                {
                    DrawChain(goBackPositions[i - 1], goBackPositions[i]);
                }
                else
                {
                    DrawChain(mountedCenter, goBackPositions[i]);
                }
            }

            return false;
        }

        private void DrawChain(Vector2 position, Vector2 position2)
        {
            bool flag11 = true;
            var mountedCenter = Main.player[projectile.owner].MountedCenter;
            var projectileChainPos = projectile.Center - Vector2.Normalize(projectile.velocity) * 8f;
            var texture = ModContent.GetTexture(this.GetPath("_Chain"));
            var frame = new Rectangle(0, 0, texture.Width, texture.Height);
            var origin = frame.Size() / 2f;
            float differenceX = position.X - position2.X;
            float differenceY = position.Y - position2.Y;
            var drawPosition = position2;
            float rotation = (position - position2).ToRotation() + MathHelper.PiOver2;
            while (flag11)
            {
                float num29 = (float)Math.Sqrt(differenceX * differenceX + differenceY * differenceY);
                if (num29 < 25f)
                {
                    flag11 = false;
                    continue;
                }
                if (float.IsNaN(num29))
                {
                    flag11 = false;
                    continue;
                }
                num29 = 12f / num29;
                differenceX *= num29;
                differenceY *= num29;
                drawPosition.X += differenceX;
                drawPosition.Y += differenceY;
                differenceX = mountedCenter.X - drawPosition.X;
                differenceY = mountedCenter.Y - drawPosition.Y;
                Main.spriteBatch.Draw(texture, new Vector2(drawPosition.X - Main.screenPosition.X, drawPosition.Y - Main.screenPosition.Y), frame, Lighting.GetColor((int)drawPosition.X / 16, (int)(drawPosition.Y / 16f)), rotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.ai[0] = -2f;
            return false;
        }
    }
}