using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Melee
{
    public class Vrang : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.scale = 0.9f;
            projectile.penetrate = -1;
            projectile.melee = true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            width = 10;
            height = 10;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough);
        }

        public override void AI()
        {
            projectile.aiStyle = -1;
            var differenceFromPlayer = Main.player[projectile.owner].Center - projectile.Center;
            float distanceFromPlayer = differenceFromPlayer.Length();
            if (distanceFromPlayer > 3000f)
            {
                projectile.Kill();
                return;
            }
            sbyte temperature = projectile.GetGlobalProjectile<AQProjectile>().temperature;
            if ((int)projectile.ai[0] <= 0)
            {
                int p = AQProjectile.FindIdentityAndType((int)projectile.ai[0], projectile.type);
                if (p == -1 || !Main.projectile[p].active)
                {
                    projectile.ai[0] = 1f;
                }
                else
                {
                    projectile.tileCollide = false;
                    projectile.velocity = Main.projectile[p].velocity;
                    return;
                }
            }
            if (Main.player[projectile.owner].itemTime < 2)
            {
                Main.player[projectile.owner].itemTime = 2;
            }
            if (Main.player[projectile.owner].itemAnimation < 2)
            {
                Main.player[projectile.owner].itemAnimation = 2;
            }
            projectile.ai[0]++;
            projectile.tileCollide = true;
            if (projectile.ai[1] == 0f)
            {
                projectile.ai[1] = projectile.velocity.Length();
            }
            float speed = projectile.ai[1];
            if (temperature > 11 || temperature < -11)
            {
                speed += temperature / 100f;
            }
            if (projectile.ai[0] > 61f)
            {
                if (Main.myPlayer == projectile.owner)
                {
                    Rectangle hitbox = projectile.getRect();
                    Rectangle plrHitbox = Main.player[projectile.owner].getRect();
                    if (hitbox.Intersects(plrHitbox))
                    {
                        projectile.Kill();
                        return;
                    }
                }
                projectile.tileCollide = false;
                speed += (projectile.ai[0] - 61f) * 0.015f;
                speed = -speed;
            }
            projectile.velocity = Vector2.Lerp(projectile.velocity, Vector2.Normalize(differenceFromPlayer) * speed, 0.01f);
            projectile.rotation += 0.4f * projectile.direction;
        }
    }
}