using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Aequus.Graphics;
using Aequus.Graphics.Primitives;
using Aequus.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Summon
{
    public class BrainCauliflowerBlast : ModProjectile
    {
        private bool _didEffects;

        public override void SetStaticDefaults()
        {
            this.SetTrail(30);
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = 360;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.extraUpdates = 5;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16 * 6;

            _didEffects = false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 80);
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            if (Projectile.velocity.Length() < 1f)
            {
                Projectile.velocity *= 0.99f;
                Projectile.alpha += 5;
                Projectile.scale -= 0.01f;
                if (Projectile.alpha > 255)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.velocity *= 0.985f;
                //if (Main.rand.NextBool(6))
                //{
                //    var d = Dust.NewDustPerfect(Projectile.Center + Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2)) * Main.rand.NextFloat(-50f, 50f) * Projectile.scale, DustID.Torch, -Projectile.velocity * 0.2f, 0, default, 1.5f);
                //    d.noGravity = true;
                //    d.fadeIn = d.scale + Main.rand.NextFloat(0.1f, 0.5f);
                //}
            }

            if (!_didEffects)
            {
                _didEffects = true;
                if (Main.netMode != NetmodeID.Server)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot.WithVolume(0.33f).WithPitchOffset(0.75f), Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item8.WithPitchOffset(-0.1f), Projectile.Center);
                }
                for (int i = 0; i < 28; i++)
                {
                    var position = Projectile.Center + (Vector2.Normalize(Projectile.velocity.RotatedBy(MathHelper.PiOver2)) * Main.rand.NextFloat(-50f, 50f) + Main.rand.NextVector2Unit() * Main.rand.NextFloat(20f)) * Projectile.scale;
                    var velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.5f, 2f);
                    if (Main.rand.NextBool(4))
                    {
                        var d = Dust.NewDustPerfect(position,
                            DustID.Torch, velocity, 0, default, Main.rand.NextFloat(1f, 1.5f));
                        d.noGravity = true;
                        d.fadeIn = d.scale + Main.rand.NextFloat(0.5f, 1f);
                        continue;
                    }

                    if (Main.rand.NextBool(4))
                    {
                        Vector2 widthMethod(float p) => new Vector2(16f) * (float)Math.Sin(p * MathHelper.Pi);
                        Color colorMethod(float p) => Color.OrangeRed.UseA(120) * 1.1f;

                        var prim = new TrailRenderer(TextureCache.Trail[0].Value, TrailRenderer.DefaultPass, widthMethod, colorMethod);
                        var particle = new BoundBowTrailParticle(prim, position, velocity,
                            new Color(200, Main.rand.Next(40) + 20, 10, 0), 1.15f, Main.rand.NextFloat(MathHelper.TwoPi), trailLength: 10, drawDust: false);
                        particle.prim.GetWidth = (p) => widthMethod(p) * particle.Scale;
                        particle.prim.GetColor = (p) => new Color(255, 180, 15, 0).HueAdd(-p * 0.15f) * (float)Math.Sin(p * MathHelper.Pi) * Math.Min(particle.Scale, 1.5f);
                        particle.Position += particle.Velocity * particle.oldPos.Length;
                        for (int k = 0; k < particle.oldPos.Length; k++)
                        {
                            particle.oldPos[k] = particle.Position - particle.Velocity * k;
                        }
                        EffectsSystem.BehindProjs.Add(particle);
                        continue;
                    }

                    EffectsSystem.BehindProjs.Add(new BloomParticle(position, velocity,
                        new Color(200, Main.rand.Next(40) + 20, 10, 0), new Color(200, 80, 10, 0), Main.rand.NextFloat(1f, 2f), 0.15f, Main.rand.NextFloat(MathHelper.TwoPi)));
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = float.NaN;
            var normal = new Vector2(1f, 0f).RotatedBy(Projectile.rotation);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(),
                Projectile.Center + normal * -46f, Projectile.Center + normal * 46f, 32f * Projectile.scale, ref _);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            int chance = 2 + (int)Projectile.ai[1] * 3;
            var aequus = Main.player[Projectile.owner].Aequus();
            if (Main.rand.NextBool(chance) && aequus.ghostSlots < aequus.ghostSlotsMax && target.lifeMax < 1000 && target.defense < 50 &&
                NecromancyDatabase.TryGet(target, out var info) && info.EnoughPower(2.1f))
            {
                var zombie = target.GetGlobalNPC<NecromancyNPC>();
                zombie.conversionChance = 1;
                zombie.renderLayer = ColorTargetID.BloodRed;
                zombie.zombieDebuffTier = 2.1f;
                zombie.zombieOwner = Projectile.owner;
                zombie.ghostDamage = Math.Max(zombie.ghostDamage, 40);
                target.StrikeNPC(2000, 1f, Projectile.direction);
                Projectile.ai[1]++;
            }
            Projectile.damage = (int)(Projectile.damage * 0.8f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.GetDrawInfo(out var texture, out var offset, out var frame, out var origin, out int trailLength);
            var bloom = TextureCache.Bloom[0].Value;

            var aura = ModContent.Request<Texture2D>(Texture + "_Aura", AssetRequestMode.ImmediateLoad).Value;
            var auraOrigin = aura.Size() / 2f;
            for (int i = 0; i < trailLength; i++)
            {
                float progress = AequusHelpers.CalcProgress(trailLength, i);
                Main.EntitySpriteDraw(aura, Projectile.oldPos[i] + offset - Main.screenPosition, null, new Color(128, 20, 10, 30) * Projectile.Opacity * progress, Projectile.oldRot[i], auraOrigin, Projectile.scale * (0.2f + progress * 0.8f), SpriteEffects.FlipHorizontally, 0);
            }
            //Main.EntitySpriteDraw(TextureCache.Bloom[0].Value, Projectile.position + offset - Main.screenPosition, null, new Color(128, 20, 10, 30) * Projectile.Opacity * 0.8f, Projectile.rotation, TextureCache.Bloom[0].Value.Size() / 2f, new Vector2(1.5f, 1f) * Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(aura, Projectile.position + offset - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.8f, Projectile.rotation, auraOrigin, new Vector2(1.5f, 1f) * Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Projectile.velocity * 8f - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.5f, SpriteEffects.FlipHorizontally, 0);
            Main.EntitySpriteDraw(texture, Projectile.position + offset - Main.screenPosition, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
            return false;
        }
    }
}