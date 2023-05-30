using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged {
    public class BoundBowProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.aiStyle = 0; // This is purposely using aiStyle 0
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 2;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(lightColor.R, lightColor.G, lightColor.B, lightColor.A - 80);
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 120;
                if (Projectile.alpha <= 0)
                {
                    Projectile.alpha = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoSparkleDust>(),
                            Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, newColor: Color.BlueViolet.UseA(25), Scale: Main.rand.NextFloat(0.65f, 1f));
                        d.fadeIn = d.scale + 0.5f;
                        d.color *= d.scale;
                    }
                }
            }
            if (Projectile.numUpdates == -1)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoSparkleDust>(),
                    -Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, newColor: Color.BlueViolet.UseA(128), Scale: Main.rand.NextFloat(0.5f, 1.25f));
                d.velocity *= 0.5f;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 4;
            height = 4;
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage = (int)(Projectile.damage * 0.5f);
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 8; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoSparkleDust>(),
                    Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, newColor: Color.BlueViolet.UseA(25), Scale: Main.rand.NextFloat(0.5f, 1f));
                d.fadeIn = d.scale + 0.5f;
                d.color *= d.scale;
            }
            for (int i = 0; i < 12; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoSparkleDust>(),
                    0f, 0f, newColor: Color.BlueViolet.UseA(25), Scale: Main.rand.NextFloat(0.5f, 1.25f));
                d.fadeIn = d.scale + 0.5f;
                d.color *= d.scale;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }
    }
}