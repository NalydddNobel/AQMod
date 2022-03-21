using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Fishing.PopperEffects
{
    public sealed class CursedPopperEffect : ModProjectile
    {
        public override string Texture => AQMod.TextureNone;

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.tileCollide = false;
            projectile.friendly = true;
            projectile.hide = true;
        }

        public override void AI()
        {
            if (projectile.ai[0] > 20f)
            {
                projectile.Kill();
            }
            if ((int)projectile.ai[0] % 4 == 0)
            {
                Dust.NewDustPerfect(projectile.Center + new Vector2(projectile.ai[0], 0f), 75, new Vector2(0f, -10f));
                Dust.NewDustPerfect(projectile.Center - new Vector2(projectile.ai[0], 0f), 75, new Vector2(0f, -10f));
            }
            projectile.ai[0]++;
        }
    }
}