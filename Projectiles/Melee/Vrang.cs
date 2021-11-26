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
            if ((int)projectile.ai[0] == 0)
            {
                projectile.velocity = Vector2.Normalize(projectile.velocity);
            }
            projectile.ai[0]++;
            if (projectile.ai[0] > 30f * Main.player[projectile.owner].meleeSpeed)
            {
                projectile.velocity += projectile.velocity * 0.001f;
            }
            else
            {
                projectile.velocity += Vector2.Normalize(projectile.velocity) * 0.0125f;
            }
        }
    }
}