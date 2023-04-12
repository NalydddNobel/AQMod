using Aequus.Common;
using Aequus.Content;
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
            Main.projFrames[Type] = 2;
            PushableEntities.AddProj(Type);
            AequusProjectile.InflictsHeatDamage.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.alpha = 200;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.scale = 0.8f;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 10;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;
            }
            Projectile.rotation += (0.7f + Projectile.velocity.Length() * 0.025f) * Projectile.direction;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = Projectile.velocity.Length();
            }

            Projectile.ai[0]++;
            float velocityMultipler = (float)Math.Sin(Math.Pow(Math.Min(Projectile.ai[0] * 0.025f, MathHelper.PiOver2) / MathHelper.PiOver2, 2) * MathHelper.PiOver2);
            Projectile.velocity = Vector2.Normalize(Projectile.velocity) * Projectile.ai[1] * velocityMultipler;
            if (Projectile.localAI[0] > 0f)
                Projectile.localAI[0]--;
            else if (Projectile.originalDamage > 0)
                Projectile.damage = Projectile.originalDamage;
            int amt = (int)Math.Max(Projectile.velocity.Length() / 3f, 1);
            var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, -Projectile.velocity.X * 0.5f, -Projectile.velocity.Y * 0.5f, Scale: Main.rand.NextFloat(1f, 2f));
            d.fadeIn = d.scale + 0.1f;
            d.noGravity = true;
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

        public void OnHit(Entity target)
        {
            new EntityCommons(target).AddBuff(BuffID.OnFire3, 360);
            Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Projectile.Center + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f)), Vector2.Normalize(Projectile.velocity) * 0.1f,
                ModContent.ProjectileType<PentalScytheExplosion>(), Projectile.damage, Projectile.knockBack * 2f, Projectile.owner, target is NPC npc ? npc.whoAmI + 1f : 0f);
            Projectile.localAI[0] = 5f;
            Projectile.damage = 0;
            Projectile.originalDamage = (int)(Projectile.originalDamage * 0.9f);
            if (Projectile.originalDamage <= 0)
            {
                Projectile.Kill();
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHit(target);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHit(target);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);

            origin.X -= 4f;
            var auraFrame = new Rectangle(frame.X, frame.Y + frame.Height, frame.Width, frame.Height);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, auraFrame, Color.Red * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.position + offset - Main.screenPosition, frame, Color.White.UseA(128) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
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
            Projectile.scale = 1f;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 100, 40, 128);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return (target.whoAmI + 1) == (int)Projectile.ai[0] ? false : null;
        }

        public override void AI()
        {
            if (Projectile.frame == 0 && Projectile.frameCounter == 0 && Main.netMode != NetmodeID.Server)
            {
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                SoundEngine.PlaySound(SoundID.Item14.WithPitchOffset(0.1f), Projectile.Center);
                for (int i = 0; i < 10; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindProjs).Setup(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(3f, 12f),
                        Color.OrangeRed.UseA(0), Color.Red.UseA(0) * Main.rand.NextFloat(0.05f, 0.3f), Main.rand.NextFloat(0.8f, 1.6f), 0.3f, Main.rand.NextFloat(MathHelper.TwoPi));
                }
                for (int i = 0; i < 15; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    ParticleSystem.New<BloomParticle>(ParticleLayer.BehindProjs).Setup(Projectile.Center + v * Main.rand.NextFloat(16f), v * Main.rand.NextFloat(1f, 5f),
                        Color.Orange.UseA(0) * Main.rand.NextFloat(0.1f, 0.4f), Color.Red.UseA(0) * Main.rand.NextFloat(0.01f, 0.05f), Main.rand.NextFloat(1.4f, 2.5f), 0.3f, Main.rand.NextFloat(MathHelper.TwoPi));
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