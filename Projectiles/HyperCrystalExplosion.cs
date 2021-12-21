using AQMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class HyperCrystalExplosion : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 80;
            projectile.height = 80;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 2;
            projectile.hide = true;
            projectile.usesIDStaticNPCImmunity = true;
            projectile.idStaticNPCHitCooldown = 2;
        }

        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                float positionLength = projectile.Center.Length() / 128f;
                float offset = MathHelper.TwoPi / 3f;
                Color color = new Color(
                                    (float)Math.Sin(positionLength) + 1f,
                                    (float)Math.Sin(positionLength + offset) + 1f,
                                    (float)Math.Sin(positionLength + offset * 2f) + 1f,
                                    0.5f);
                var type = ModContent.DustType<MonoDust>();
                for (int i = 0; i < 80; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, type, default, default, default, color, 1.65f);
                }
                Main.PlaySound(SoundID.Item14, projectile.Center);
                projectile.ai[0] = 1f;
            }
        }
    }
}
