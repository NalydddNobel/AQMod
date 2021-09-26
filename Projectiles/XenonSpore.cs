using AQMod.Common;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Projectiles
{
    public class XenonSpore : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.timeLeft = 360;
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == 0)
            {
                projectile.velocity *= 0.95f;
                if (projectile.velocity.Length() < 0.1f)
                {
                    projectile.velocity = new Vector2(0f, 0f);
                    projectile.ai[0] = 1f;
                }
                if (Main.rand.NextBool(12))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.XenonDust>());
                    Main.dust[d].velocity.X *= 0.5f;
                    Main.dust[d].velocity.Y = Main.dust[d].velocity.Y.Abs() * -1.1f;
                }
            }
            else
            {
                int target = AQNPC.FindClosest(projectile.Center, 400f);
                if (target == -1)
                {
                    projectile.Kill();
                    return;
                }
                projectile.ai[0] += 0.05f;
                projectile.velocity = Vector2.Lerp(projectile.velocity, new Vector2(projectile.ai[0], 0f).RotatedBy((Main.npc[target].Center - projectile.Center).ToRotation()), projectile.ai[0] * 0.005f);
                if (Main.rand.NextBool(4))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.XenonDust>());
                    Main.dust[d].velocity.X *= 0.5f;
                    Main.dust[d].velocity.Y = Main.dust[d].velocity.Y.Abs() * -1.1f;
                }
                if (Main.rand.NextBool(4))
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<Dusts.XenonMist>());
                }
            }
            Lighting.AddLight(projectile.Center, new Vector3(0.05f, 0.05f, 0.1f));
            projectile.gfxOffY = (float)Math.Sin(projectile.timeLeft * 0.0628f);
        }

        public override bool CanDamage() => (int)projectile.ai[0] != 0;

        public override void Kill(int timeLeft)
        {
            var dustType = ModContent.DustType<Dusts.XenonDust>();
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 0, default(Color), 1.4f);
            }
        }
    }
}