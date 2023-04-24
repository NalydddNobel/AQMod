using Aequus.Buffs;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Net.Sounds;
using Aequus.Items.Weapons.Melee.Heavy;
using Aequus.Particles;
using Aequus.Particles.Dusts;
using Aequus.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Melee.Swords {
    public class UltimateSwordProj : SwordProjectileBase
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 70;
            ProjectileID.Sets.TrailingMode[Type] = -1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 160;
            Projectile.height = 160;
            Projectile.extraUpdates = 10;
            Projectile.localNPCHitCooldown *= 10;
            swordHeight = 100;
            rotationOffset = -MathHelper.PiOver4 * 3f;
            Projectile.noEnchantmentVisuals = true;
            amountAllowedToHit = 5;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        protected override void Initialize(Player player, AequusPlayer aequus)
        {
            base.Initialize(player, aequus);
            if (aequus.itemCombo > 0)
            {
                swingDirection *= -1;
            }
        }

        public override void AI()
        {
            _halfWayMark = true;
            base.AI();
            if (Main.player[Projectile.owner].itemAnimation <= 1)
            {
                Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 20 : 0);
            }
            if (!playedSound && AnimProgress > 0.4f)
            {
                playedSound = true;
                SoundEngine.PlaySound(AequusSounds.swordSwoosh with { Pitch = 0.4f, }, Projectile.Center);
            }
        }

        public override Vector2 GetOffsetVector(float progress)
        {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - (MathHelper.PiOver2 * 1.5f)) * -swingDirection * 1.1f);
        }

        public Color GetAuraColor(float offset) {
            float time = (Main.GameUpdateCount / 10f + offset) * MathHelper.TwoPi;

            return new(MathF.Sin(time) * 0.66f, MathF.Pow(Math.Abs(MathF.Sin(time + MathHelper.PiOver2)), 2f) * 0.8f + 0.2f, Math.Abs(MathF.Sin(time)));
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress)
        {
            if (progress > 0.85f)
            {
                Projectile.Opacity = 1f - (progress - 0.85f) / 0.15f;
            }

            if (progress > 0.25f && progress < 0.65f)
            {
                if (Projectile.numUpdates <= 3)
                {
                    var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 6f) + Main.player[Projectile.owner].velocity;
                    var d = Dust.NewDustPerfect(
                        Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, swordHeight * Projectile.scale * baseSwordScale),
                        ModContent.DustType<MonoDust>(),
                        velocity,
                        newColor: GetAuraColor(Main.rand.NextFloat(0.1f)) with { A = 128 },
                        Scale: MathF.Pow(baseSwordScale * 0.9f, 2f)
                    );
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.color *= d.scale;
                    d.noGravity = true;

                    if (progress > 0.35f && progress < 0.55f && Projectile.numUpdates < 2) {
                        var flashColor = GetAuraColor(Main.rand.NextFloat(0.1f));
                        var particle = ParticleSystem.New<ShinyFlashParticle>(ParticleLayer.BehindProjs)
                            .Setup(
                                Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(swordHeight / 2f * baseSwordScale, swordHeight * baseSwordScale),
                                velocity,
                                flashColor with { A = 100 },
                                flashColor.SaturationMultiply(0.5f) with { A = 0, } * 0.33f,
                                Main.rand.NextFloat(0.4f, 0.7f) * baseSwordScale,
                                0.2f,
                                Main.rand.NextFloat(MathHelper.TwoPi)
                            );
                        particle.flash -= Main.rand.Next(6);
                    }

                    if (Projectile.numUpdates == -1) {
                        AequusPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                    }
                }
            }

            Projectile.oldPos[0] = AngleVector * 60f * Projectile.scale;
            Projectile.oldRot[0] = Projectile.oldPos[0].ToRotation() + MathHelper.PiOver4;

            // Manually updating oldPos and oldRot 
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                Projectile.oldPos[i] = Projectile.oldPos[i - 1];
                Projectile.oldRot[i] = Projectile.oldRot[i - 1];
            }
        }

        public override float SwingProgress(float progress)
        {
            return GenericSwing2(progress);
        }
        public override float GetScale(float progress)
        {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f) {
                return scale + 0.25f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }
        public override float GetVisualOuter(float progress, float swingProgress)
        {
            return 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            AequusBuff.ApplyBuff<AethersWrath>(target, 360, out bool canPlaySound);
            if (canPlaySound)
            {
                ModContent.GetInstance<AethersWrathSound>().Play(target.Center);
            }
            if (canPlaySound || target.HasBuff<AethersWrath>())
            {
                for (int i = 0; i < 12; i++)
                {
                    var v = Main.rand.NextVector2Unit();
                    var d = Dust.NewDustPerfect(target.Center + v * new Vector2(Main.rand.NextFloat(target.width / 2f + 16f), Main.rand.NextFloat(target.height / 2f + 16f)), DustID.AncientLight, v * 8f);
                    d.noGravity = true;
                    d.noLightEmittence = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var auraColor = GetAuraColor(0f);
            var texture = TextureAssets.Projectile[Type].Value;
            var center = Main.player[Projectile.owner].Center;
            var handPosition = Main.GetPlayerArmPosition(Projectile) + AngleVector * animationGFXOutOffset;
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            var drawCoords = handPosition - Main.screenPosition;
            float size = texture.Size().Length();
            var effects = SpriteEffects.None;
            bool flip = Main.player[Projectile.owner].direction == 1 ? combo > 0 : combo == 0;
            var glowmask = AequusTextures.UltimateSwordProj_Glow.Value;
            if (flip)
            {
                Main.instance.LoadItem(ModContent.ItemType<UltimateSword>());
                texture = TextureAssets.Item[ModContent.ItemType<UltimateSword>()].Value;
                glowmask = AequusTextures.UltimateSword_Glow.Value;
            }
            float animProgress = AnimProgress;
            float swishProgress = 0f;
            float intensity = 0f;
            if (animProgress > 0.3f && animProgress < 0.65f) {
                swishProgress = (animProgress - 0.3f) / 0.35f;
                intensity = (float)Math.Sin(MathF.Pow(swishProgress, 2f) * MathHelper.Pi);
            }

            var origin = new Vector2(0f, texture.Height);

            var bloom = AequusTextures.Bloom1;
            Main.EntitySpriteDraw(bloom, handPosition + AngleVector * 60f * Projectile.scale - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.5f, Projectile.rotation + MathHelper.PiOver4, bloom.Size() / 2f, new Vector2(Projectile.scale * 0.3f, Projectile.scale), effects, 0);

            var circular = Helper.CircularVector(4, Projectile.rotation);
            for (int i = 0; i < circular.Length; i++)
            {
                Vector2 v = circular[i];
                Main.EntitySpriteDraw(texture, drawCoords + v * 2f * Projectile.scale, null, GetAuraColor(i * 0.25f) with { A = 0 } * 0.33f * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            }

            float trailAlpha = 1f;
            for (float f = lastAnimProgress; f > 0f && f < 1f && trailAlpha > 0f; f += -0.01f) {
                InterpolateSword(f, out var offsetVector, out float _, out float scale, out float outer);
                Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, GetAuraColor((1f - trailAlpha) * 0.25f) with { A = 0 } * Projectile.Opacity * 0.4f * trailAlpha * intensity, (handPosition - (handPosition + offsetVector * swordHeight)).ToRotation() + rotationOffset, origin, scale, effects, 0);
                trailAlpha -= 0.07f;
            }

            Main.EntitySpriteDraw(texture, drawCoords, null, Projectile.GetAlpha(lightColor) * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);
            Main.EntitySpriteDraw(glowmask, drawCoords, null, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale, effects, 0);

            if (intensity > 0f)
            {
                float progress2 = 1f - (float)Math.Pow(1f - swishProgress, 2f);
                Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor.UseA(0) * intensity * 0.5f, Projectile.rotation, origin, Projectile.scale, effects, 0);

                var swish = AequusTextures.Swish.Value;
                var swishOrigin = swish.Size() / 2f;
                var swishColor = auraColor.UseA(58) * 0.33f * intensity * intensity * Projectile.Opacity;
                float r = BaseAngleVector.ToRotation();
                var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 40f + 20f * swishProgress) * baseSwordScale, null, swishColor, r + MathHelper.PiOver2, swishOrigin, new Vector2(1.8f, 1.8f), effects, 0);
                Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 60f) * baseSwordScale, null, swishColor * 0.4f, r + MathHelper.PiOver2, swishOrigin, new Vector2(2f, 3f), effects, 0);
            }
            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(Projectile.scale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Projectile.scale = reader.ReadSingle();
        }
    }
}