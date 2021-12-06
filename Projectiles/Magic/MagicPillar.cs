using AQMod.Common.Graphics.Particles;
using AQMod.Common.Graphics.Particles;
using AQMod.Content.Dusts;
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
            int type = ModContent.DustType<MonoDust>();
            var clr = new Color(255, 150, 150, 0);
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
                        ParticleLayers.AddParticle_PostDrawPlayers(
                           new MonoParticleEmber(new Vector2(x, projectile.position.Y + Main.rand.NextFloat(projectile.height)), new Vector2(-projectile.velocity.X * 0.25f, Main.rand.NextFloat(-0.1f, 0.1f)),
                           new Color(255, 150, 150, 0), Main.rand.NextFloat(0.6f, 1.1f)));
                    }
                    var position = new Vector2(x, projectile.position.Y + Main.rand.NextFloat(projectile.height));
                    float scale = Main.rand.NextFloat(0.6f, 1.1f);
                    ParticleLayers.AddParticle_PostDrawPlayers(
                        new MonoParticleEmber(position, new Vector2(-projectile.velocity.X * 0.25f, Main.rand.NextFloat(-0.1f, 0.1f)),
                        new Color(255, 222, 222, 0), scale));
                    ParticleLayers.AddParticle_PostDrawPlayers(
                        new MonoParticleEmber(position, new Vector2(-projectile.velocity.X * 0.25f, Main.rand.NextFloat(-0.1f, 0.1f)),
                        new Color(100, 60, 60, 0), scale * 1.65f));
                    ParticleLayers.AddParticle_PostDrawPlayers(
                        new MonoParticleEmber(new Vector2(x, projectile.position.Y + Main.rand.NextFloat(projectile.height)), new Vector2(-projectile.velocity.X * 0.25f, Main.rand.NextFloat(-0.1f, 0.1f)),
                        new Color(255, 180, 180, 0), Main.rand.NextFloat(1f, 2.6f)));
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
