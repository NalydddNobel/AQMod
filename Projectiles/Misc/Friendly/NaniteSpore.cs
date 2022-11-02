using Aequus.Graphics;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Misc.Friendly
{
    public class NaniteSpore : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.SporeTrap);
            Projectile.width = 80;
            Projectile.height = 80;
            AIType = ProjectileID.SporeTrap;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * Projectile.Opacity * AequusHelpers.Wave(Main.GlobalTimeWrappedHourly * 5f, 0.8f, 1f));
        }

        public override void Kill(int timeLeft)
        {
            if (Main.netMode == NetmodeID.Server || Projectile.penetrate != 0)
            {
                return;
            }

            if (Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Normalize(Projectile.velocity), ModContent.ProjectileType<NaniteExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            for (int i = 0; i < 20; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
            for (int i = 0; i < 10; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke);
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var t, out var off, out var frame, out var origin, out int trailLength);

            Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(t, Projectile.position + off - Main.screenPosition, frame, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            var trailColor = new Color(50, 50, 50, 0);
            for (int i = 0; i < trailLength; i++)
            {
                float p = AequusHelpers.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(t, Projectile.oldPos[i] + off - Main.screenPosition, frame, trailColor * p * Projectile.Opacity, Projectile.oldRot[i], origin, Projectile.scale * (0.8f + 0.2f * p), SpriteEffects.None, 0f);
            }
            return false;
        }
    }

    public class NaniteExplosion : ModProjectile
    {
        public override string Texture => "Aequus/Assets/Explosion1";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToExplosion(90, DamageClass.Ranged, 20);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(10, 5, 50, 0);
        }

        public override void AI()
        {
            if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 8; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    EffectsSystem.BehindPlayers.Add(new BloomParticle(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(3f, 12f),
                        new Color(4, 15, 25, 0), new Color(10, 15, 50, 0), 1.25f, 0.3f));
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 2)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Type])
                {
                    Projectile.hide = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int _);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}