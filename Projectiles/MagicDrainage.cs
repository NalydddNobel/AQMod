using AQMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public sealed class MagicDrainage : ModProjectile
    {
        public override string Texture => AQMod.TextureNone;

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 240;
            projectile.extraUpdates = 5;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            if (!projectile.active)
            {
                return;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] < 45f)
            {
                projectile.velocity = projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.075f, 0.075f));
                projectile.velocity *= 1.01f;
            }
            else
            {
                var difference = Main.player[projectile.owner].Center - projectile.Center;
                if (difference.Length() < Main.player[projectile.owner].width)
                {
                    projectile.Kill();
                    projectile.velocity = difference;
                }
                else
                {
                    float additional = (projectile.ai[0] - 45f) / 900f;
                    projectile.velocity = Vector2.Lerp(projectile.velocity, difference * additional, additional * 0.75f);
                }
            }
            float separation = 2f;
            var v = Vector2.Normalize(projectile.velocity) * separation;
            var center = projectile.Center;
            float maxLength = Math.Max(projectile.velocity.Length(), 2.5f) / 2;
            float alpha = 1f - projectile.alpha / 255f;
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 3;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
            }
            for (int i = 0; i < maxLength; i++)
            {
                Dust.NewDustPerfect(center + v * i, ModContent.DustType<MonoDust>(), Vector2.Zero, 0, new Color(50, 75, 255, 0) * alpha, 2f);
            }
        }

        public override void Kill(int timeLeft)
        {
            int manaHealed = (int)projectile.ai[1];
            if (manaHealed <= 0)
            {
                return;
            }
            if (Main.myPlayer == projectile.owner)
            {
                Main.player[projectile.owner].ManaEffect(manaHealed);
            }
            Main.player[projectile.owner].statMana = Math.Min(Main.player[projectile.owner].statMana + manaHealed, Main.player[projectile.owner].statManaMax2);
        }
    }
}