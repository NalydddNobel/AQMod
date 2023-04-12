using Aequus.Buffs.Necro;
using Aequus.Common.Primitives;
using Aequus.Content;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon.Necro
{
    public class ZombieBolt : ModProjectile
    {
        protected TrailRenderer prim;
        protected float primScale;
        protected Color primColor;

        public virtual float Tier => 1f;

        public override void SetStaticDefaults()
        {
            PushableEntities.AddProj(Type);
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.scale = 0.8f;
            Projectile.alpha = 10;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.DamageType = NecromancyDamageClass.Instance;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(100, 140, 255, 255 - Projectile.alpha);
        }

        public override void AI()
        {
            if (Main.netMode != NetmodeID.Server)
            {
                var center = Projectile.Center;
                foreach (var v in Helper.CircularVector(3, Main.GlobalTimeWrappedHourly * 5f))
                {
                    if (Main.rand.NextBool(3))
                        ParticleSystem.New<BloomParticle>(ParticleLayer.BehindProjs).Setup(center + v * 4f, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * -0.125f, new Color(140, 130, 255, 100), Color.Blue.UseA(0) * 0.1f, 1.1f, 0.35f, Main.rand.NextFloat(MathHelper.TwoPi));
                }
            }

            int target = Projectile.FindTargetWithLineOfSight(300f);
            if (target != -1)
            {
                float speed = Projectile.velocity.Length();
                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(Main.npc[target].Center) * speed, 0.075f)) * speed;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            NecromancyDebuff.ReduceDamageForDebuffApplication<NecromancyDebuff>(Tier, target, ref modifiers);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
            NecromancyDebuff.ApplyDebuff<NecromancyDebuff>(target, 600, Projectile.owner);
        }

        public override void Kill(int timeLeft)
        {
            var center = Projectile.Center;
            for (int i = 0; i < 12; i++)
            {
                var d = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<MonoDust>(), newColor: new Color(222, 210, 255, 150));
                d.velocity *= 0.2f;
                d.velocity += (d.position - center) / 8f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var texture = TextureAssets.Projectile[Type].Value;
            var frame = Projectile.Frame();
            var origin = frame.Size() / 2f;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, new Color(10, 40, 255, 100), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            return false;
        }

        protected void DrawTrail(float waveSize = 8f, int maxLength = -1, float timeMultiplier = 5f)
        {
            if (prim == null)
            {
                prim = new TrailRenderer(TrailTextures.Trail[0].Value, TrailRenderer.DefaultPass, (p) => new Vector2(primScale) * (1f - p), (p) => primColor * (1f - p));
            }

            int trailLength = maxLength > 0 ? maxLength : ProjectileID.Sets.TrailCacheLength[Type];
            var trail = new Vector2[trailLength];
            var offset = new Vector2(Projectile.width / 2f, Projectile.height / 2f);
            for (int i = 0; i < trailLength; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    trailLength = i;
                    continue;
                }
                trail[i] = Projectile.oldPos[i] + offset;
            }
            foreach (var f in Helper.Circular(3, Main.GlobalTimeWrappedHourly * timeMultiplier))
            {
                var renderTrail = new Vector2[trailLength];
                Array.Copy(trail, renderTrail, trailLength);

                for (int i = 0; i < trailLength; i++)
                {
                    renderTrail[i] += new Vector2(0f, (float)Math.Sin(f + i * 0.33f) * waveSize).RotatedBy(Projectile.oldRot[i]);
                }

                prim.Draw(renderTrail);
            }
        }
    }
}