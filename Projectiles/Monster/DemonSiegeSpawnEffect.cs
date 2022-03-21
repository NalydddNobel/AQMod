using AQMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Monster
{
    public sealed class DemonSiegeSpawnEffect : ModProjectile
    {
        public override string Texture => AQMod.TextureNone;

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.tileCollide = false;
            projectile.hide = true;
        }

        public override void AI()
        {
            float offX = (float)Math.Sin((60 - projectile.ai[0]) * 0.1f) * projectile.ai[1];
            offX *= projectile.ai[0] % 2 == 0 ? 1 : -1;
            var spawnPosition = new Vector2(projectile.position.X + projectile.width / 2f + offX, projectile.position.Y + projectile.height / 2f);
            int d = Dust.NewDust(spawnPosition, 2, 2, ModContent.DustType<DemonSpawnDust>());
            Main.dust[d].velocity.X *= 0.5666f;
            Main.dust[d].velocity.Y -= 3.666f;
            projectile.ai[0]--;
            if (projectile.ai[0] < 0)
            {
                Main.PlaySound(SoundID.DD2_BetsyFlameBreath.WithVolume(0.6f), projectile.Center);
                projectile.Kill();
            }
        }
    }
}