using AQMod.Dusts;
using AQMod.Effects.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Projectiles.Magic
{
    public class MagicPillar : ModProjectile
    {
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.aiStyle = -1;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.hide = true;
            projectile.extraUpdates = 2;
            projectile.timeLeft = 30 * (projectile.extraUpdates + 1);
        }

        public override void AI()
        {
            if ((int)projectile.ai[0] == 0)
            {
                projectile.ai[0] = 1f;
                projectile.velocity /= (projectile.extraUpdates + 1);
            }
            if (projectile.timeLeft < 60 && projectile.alpha < 240)
            {
                projectile.alpha += 2;
            }
            if (projectile.alpha > 255)
            {
                return;
            }
            float alpha = 1f - projectile.alpha / 255f;
            int type = ModContent.DustType<MonoDust>();
            var clr = new Color(200, 100, 255, 0) * alpha;
            if (AQConfigClient.c_EffectQuality >= 1f)
            {
                float x = projectile.position.X;
                if (projectile.direction == 1)
                    x += projectile.width;
                int d = Dust.NewDust(new Vector2(x, projectile.position.Y), 2, projectile.height, type, 0, 0, 0, clr, Main.rand.NextFloat(0.8f, 1.2f));
                Main.dust[d].velocity.X = -projectile.velocity.X * 0.1f;
                Main.dust[d].velocity.Y *= 0.1f;
                if (Main.netMode != NetmodeID.Server && AQConfigClient.c_EffectQuality >= 1f)
                {
                    for (int i = 0; i < 7 * AQConfigClient.c_EffectQuality; i++)
                    {
                        Particle.PostDrawPlayers.AddParticle(
                           new EmberParticle(new Vector2(x, projectile.position.Y + Main.rand.NextFloat(projectile.height)), new Vector2(-projectile.velocity.X * 0.25f, Main.rand.NextFloat(-0.1f, 0.1f)),
                           new Color(200, 100, 255, 0) * alpha, Main.rand.NextFloat(0.6f, 1.1f)));
                    }
                    var position = new Vector2(x, projectile.position.Y + Main.rand.NextFloat(projectile.height));
                    float scale = Main.rand.NextFloat(0.6f, 1.1f);
                    Particle.PostDrawPlayers.AddParticle(
                        new SparkleParticle(position, new Vector2(-projectile.velocity.X * 0.25f, Main.rand.NextFloat(-0.1f, 0.1f)),
                        new Color(222, 150, 255, 0) * alpha, scale));
                }
                for (int i = 0; i < 3; i++)
                {
                    int d1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0, 0, 0, clr, 0.9f);
                    Main.dust[d].rotation = 0f;
                    Main.dust[d].velocity.Y *= 0.25f;
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, type, 0, 0, 0, clr, 2f);
            }
        }
    }
}
