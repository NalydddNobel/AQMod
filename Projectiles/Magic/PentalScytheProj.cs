using Aequus.Graphics;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Magic
{
    public class PentalScytheProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            this.SetTrail(10);
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.alpha = 200;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            Projectile.rotation += (0.35f + Projectile.velocity.Length() * 0.025f) * Projectile.direction;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = Projectile.velocity.Length();
            }

            Projectile.ai[0]++;
            float velocityMultipler = (float)Math.Sin(Math.Pow(Math.Min(Projectile.ai[0] * 0.025f, MathHelper.PiOver2) / MathHelper.PiOver2, 2) * MathHelper.PiOver2);
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * Projectile.ai[1] * velocityMultipler;

            int amt = (int)Math.Max(Projectile.velocity.Length() / 3f, 1);
            for (int i = 0; i < 2; i++)
            {
                var d = Dust.NewDustDirect(Projectile.Center + new Vector2(Projectile.width / 2f * Main.rand.NextFloat(), 0f).RotatedBy(Main.rand.NextFloat(-1f, 1f) + Projectile.rotation), 8, 8, DustID.Torch);
                d.scale *= Main.rand.NextFloat(1f, 1.525f);
                d.velocity = Vector2.Normalize(d.position - Projectile.Center).RotatedBy(MathHelper.PiOver2 * Projectile.direction) * d.velocity.Length() * Projectile.velocity.Length() / 3f * d.scale + Projectile.velocity;
                d.noGravity = true;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            fallThrough = true;
            return true;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f)), Vector2.Normalize(Projectile.velocity) * 0.1f,
                ModContent.ProjectileType<PentalScytheExplosion>(), Projectile.damage, Projectile.knockBack * 2f, Projectile.owner);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire3, 360);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f)), Vector2.Normalize(Projectile.velocity) * 0.1f, 
                ModContent.ProjectileType<PentalScytheExplosion>(), Projectile.damage, Projectile.knockBack * 2f, Projectile.owner, target.whoAmI + 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);

            var auraTexture = ModContent.Request<Texture2D>(Texture + "_Aura").Value;
            float opacity = Math.Clamp(Projectile.velocity.Length() / 20f, 0.2f, 1f);
            var auraOrigin = auraTexture.Size() / 2f;
            for (int i = 0; i < trailLength; i++)
            {
                float p = AequusHelpers.CalcProgress(trailLength, i);
                Main.spriteBatch.Draw(auraTexture, Projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(120, 50, 33, 0) * opacity * p, Projectile.oldRot[i], auraOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            float offsetRotation = 10f;
            Main.spriteBatch.Draw(auraTexture, Projectile.position + offset + new Vector2(offsetRotation, 0f).RotatedBy(Projectile.rotation) - Main.screenPosition, null, new Color(120, 50, 33, 0) * opacity, Projectile.rotation, auraOrigin, Projectile.scale, SpriteEffects.None, 0f);
            float amt = AequusHelpers.Wave(Projectile.rotation % MathHelper.TwoPi, 0.5f, 1f) * opacity;
            var color = Color.Lerp(Color.Orange, Color.White, amt);
            Main.spriteBatch.Draw(texture, Projectile.position + offset + new Vector2(offsetRotation, 0f).RotatedBy(Projectile.rotation) - Main.screenPosition, frame, color * 0.2f, Projectile.rotation - 0.2f, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset + new Vector2(offsetRotation, 0f).RotatedBy(Projectile.rotation + 0.2f) - Main.screenPosition, frame, color * 0.2f, Projectile.rotation + 0.2f, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset + new Vector2(offsetRotation, 0f).RotatedBy(Projectile.rotation - 0.2f) - Main.screenPosition, frame, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }

    public class PentalScytheExplosion : ModProjectile
    {
        public override string Texture => "Aequus/Assets/Explosion1";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToExplosion(90, DamageClass.Magic, 20);
            Projectile.scale = 1.2f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.OrangeRed.UseA(150) * 0.8f;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return (target.whoAmI + 1) == (int)Projectile.ai[0] ? false : null;
        }

        public override void AI()
        {
            if (Projectile.frame == 0 && Main.netMode != NetmodeID.Server)
            {
                SoundEngine.PlaySound(SoundID.Item14.WithPitchOffset(0.1f), Projectile.Center);
                for (int i = 0; i < 10; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    AequusEffects.BehindProjs.Add(new BloomParticle(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(3f, 12f),
                        Color.Yellow.UseA(0) * Main.rand.NextFloat(0.3f, 0.7f), Color.Red.UseA(0) * Main.rand.NextFloat(0.05f, 0.15f), Main.rand.NextFloat(0.8f, 1.6f), 0.3f, Main.rand.NextFloat(MathHelper.TwoPi)));
                }
                for (int i = 0; i < 15; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    AequusEffects.BehindProjs.Add(new BloomParticle(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(1f, 5f),
                        Color.Orange.UseA(0) * Main.rand.NextFloat(0.1f, 0.4f), Color.Red.UseA(0) * Main.rand.NextFloat(0.01f, 0.05f), Main.rand.NextFloat(1.4f, 2.5f), 0.3f, Main.rand.NextFloat(MathHelper.TwoPi)));
                }
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
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