using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class SoulAbsorbtion : ModProjectile
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
            Projectile.extraUpdates = 5;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            if (!Projectile.active)
            {
                return;
            }
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 45f)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.075f, 0.075f));
                Projectile.velocity *= 1.01f;
            }
            else
            {
                var difference = Main.player[Projectile.owner].Center - Projectile.Center;
                if (difference.Length() < Main.player[Projectile.owner].width)
                {
                    Projectile.Kill();
                    Projectile.velocity = difference;
                }
                else
                {
                    float additional = (Projectile.ai[0] - 45f) / 900f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, difference * additional, additional * 0.75f);
                }
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