using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro {
    public class SoulAbsorbProj : ModProjectile
    {
        public override string Texture => Aequus.BlankTexture;

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.extraUpdates = 1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (!Projectile.active)
            {
                return;
            }
            Projectile.ai[0]++;
            var difference = Main.player[Projectile.owner].Center - Projectile.Center;
            if (difference.Length() < Main.player[Projectile.owner].width)
            {
                Projectile.Kill();
                Projectile.velocity = difference;
            }
            else
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, difference / 16f, 0.1f);
            }
            float separation = 2f;
            var v = Vector2.Normalize(Projectile.velocity) * separation;
            var center = Projectile.Center;
            float maxLength = Math.Max(Projectile.velocity.Length(), 2.5f) / 2;
            float alpha = 1f - Projectile.alpha / 255f;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 3;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }
            for (int i = 0; i < maxLength; i++)
            {
                Dust.NewDustPerfect(center + v * i, ModContent.DustType<MonoDust>(), Vector2.Zero, 0, new Color(255, 50, 100, 0) * alpha, 2f);
            }
        }
    }
}