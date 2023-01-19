using Aequus.Content;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Ranged
{
    public class SnowflakeCannonProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.NorthPoleSnowflake;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = Main.projFrames[ProjectileID.NorthPoleSnowflake];

            this.SetTrail(12);

            PushableEntities.ProjectileIDs.Add(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.scale = 1.5f;
            Projectile.coldDamage = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            if ((int)Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1f;
                Projectile.ai[0] = Projectile.velocity.X;
                Projectile.ai[1] = Projectile.velocity.Y;
                Projectile.frame = Main.rand.Next(Main.projFrames[Type]);
                Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            }
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
            }

            var normalVelocity = new Vector2(Projectile.ai[0], Projectile.ai[1]);
            float speed = normalVelocity.Length();
            float time = Math.Min(Projectile.localAI[0] * 0.15f, MathHelper.Pi);
            speed *= 1f + 1f * (float)Math.Sin(time);
            Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, normalVelocity, 0.1f)) * speed;
            Projectile.localAI[0]++;

            if (time < MathHelper.Pi)
            {
                int target = Projectile.FindTargetWithLineOfSight(500f);
                if (target == -1)
                {
                    if ((int)Projectile.localAI[1] == 0)
                    {
                        Projectile.localAI[1] = Main.rand.NextBool() ? -1f : 1f;
                    }
                    Projectile.velocity = Projectile.velocity.RotatedBy(((float)Math.Sin(time * 1.5f) - 0.5f) * 0.15f * Projectile.localAI[1]);
                }
                else
                {
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * speed, 0.2f);
                }
            }
            if (Main.rand.NextBool(12))
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
            }
            Projectile.rotation += Projectile.velocity.Length() * 0.01f;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, crit ? 480 : 240);
            Projectile.velocity = -Projectile.velocity;
            Projectile.localAI[0] = 1f;
            for (int i = 0; i < 5; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(15, 120 + Main.rand.Next(40), 222, 0));
                d.velocity = (d.position - Projectile.Center) / 2f;
            }
        }
        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Frostburn, crit ? 480 : 240);
            Projectile.velocity = -Projectile.velocity;
            Projectile.localAI[0] = 1f;
            for (int i = 0; i < 5; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(15, 120 + Main.rand.Next(40), 222, 0));
                d.velocity = (d.position - Projectile.Center) / 2f;
            }
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item51, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), 0f, 0f, 0, new Color(15, 120 + Main.rand.Next(40), 222, 0));
                d.velocity = (d.position - Projectile.Center) / 2f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            var trailColor = new Color(15, 120, 222, 0) * (1f - Projectile.alpha / 255f);
            var origin = frame.Size() / 2f;
            this.DrawTrail((v, progress) =>
            {
                float wave = AequusHelpers.Wave(progress * 2f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 10f, 0f, 1f);
                Main.EntitySpriteDraw(texture, v - Main.screenPosition, frame, Color.Lerp(new Color(80, 180, 222, 0), new Color(1, 111, 255, 80), wave) * progress, Projectile.rotation, origin, Projectile.scale + 0.1f * (1f - wave), SpriteEffects.None, 0);
            });
            var drawCoordinates = Projectile.Center - Main.screenPosition;
            foreach (var v in AequusHelpers.CircularVector(4, Projectile.rotation))
            {
                Main.EntitySpriteDraw(texture, drawCoordinates + v * 2f, frame, Color.Lerp(new Color(80, 180, 222, 0), new Color(1, 111, 255, 80), AequusHelpers.Wave(MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 10f, 0f, 1f)), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture, drawCoordinates, frame, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}