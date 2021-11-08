using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public sealed class FriendlyWind : ModProjectile
    {
        public static int NewWind(Player player, Vector2 position, Vector2 velocity, float windSpeed, int lifeSpan = 300, int size = 40)
        {
            int p = Projectile.NewProjectile(position, velocity, ModContent.ProjectileType<FriendlyWind>(), 0, windSpeed, player.whoAmI);
            Main.projectile[p].width = size;
            Main.projectile[p].height = size;
            Main.projectile[p].timeLeft = lifeSpan;
            return p;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.hide = true;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            if (Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.velocity *= 0.97f;
            }
            for (int i = 0; i < Main.maxNPCs;i++)
            {
                var target = Main.npc[i];
                if (target.active)
                {
                    if (!target.noGravity && projectile.getRect().Intersects(target.getRect()))
                    {
                        target.velocity += Vector2.Normalize(projectile.velocity) * projectile.knockBack;
                    }
                }
            }
        }
    }
}