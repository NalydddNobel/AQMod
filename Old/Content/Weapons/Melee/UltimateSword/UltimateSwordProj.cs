using Aequus.Common.Buffs;
using Aequus.Common.Projectiles;
using Aequus.Old.Content.Particles;
using System;
using System.IO;
using Terraria.Audio;

namespace Aequus.Old.Content.Weapons.Melee.UltimateSword;

public class UltimateSwordProj : LegacyHeldSlashingSwordProjectile {
    public override string Texture => AequusTextures.UltimateSword.Path;

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 160;
        Projectile.height = 160;
        Projectile.extraUpdates = 10;
        Projectile.localNPCHitCooldown *= 10;
        swordHeight = 110;
        Projectile.noEnchantmentVisuals = true;
        hitsLeft = 5;
        gfxOutOffset = -12;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override void AI() {
        base.AI();
        //if (Main.player[Projectile.owner].itemAnimation <= 1) {
        //    Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().itemCombo = (ushort)(combo == 0 ? 20 : 0);
        //}
        if (!playedSound && AnimProgress > 0.4f) {
            playedSound = true;
            SoundEngine.PlaySound(AequusSounds.HeavySwing with { Pitch = 0.4f, }, Projectile.Center);
        }
    }

    public override Vector2 GetOffsetVector(float progress) {
        return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - MathHelper.PiOver2 * 1.5f) * -swingDirection * 1.1f);
    }

    public Color GetAuraColor(float offset) {
        float time = (Main.GameUpdateCount / 11f + offset) * MathHelper.TwoPi;

        return new(MathF.Sin(time) * 0.66f, MathF.Pow(Math.Abs(MathF.Sin(time + MathHelper.PiOver2)), 2f) * 0.8f + 0.2f, Math.Abs(MathF.Sin(time)));
    }

    public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
        if (progress > 0.25f && progress < 0.65f) {
            if (Projectile.numUpdates < 5) {
                var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 6f) + Main.player[Projectile.owner].velocity;
                var d = Dust.NewDustPerfect(
                    Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, swordHeight * Projectile.scale * baseSwordScale),
                    ModContent.DustType<MonoDust>(),
                    velocity,
                    newColor: GetAuraColor(Main.rand.NextFloat(0.1f)) with { A = 0 },
                    Scale: Main.rand.NextFloat(0.5f, 2f)
                );
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                d.color *= d.scale;
                d.noGravity = true;

                //if (Projectile.numUpdates < 1 && progress > 0.35f && progress < 0.6f) {
                //    var flashColor = GetAuraColor(Main.rand.NextFloat(0.1f));
                //    var particle = ParticleSystem.New<MonoFlashParticle>(ParticleLayer.AboveDust)
                //        .Setup(
                //            Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(swordHeight * 0.5f * baseSwordScale, swordHeight * baseSwordScale * 1.2f),
                //            Vector2.Zero,
                //            flashColor.SaturationMultiply(0.4f) with { A = 100 },
                //            flashColor with { A = 0, } * 0.2f,
                //            Main.rand.NextFloat(0.45f, 0.65f) * baseSwordScale,
                //            0.8f,
                //            0f
                //        );
                //    particle.flash += Main.rand.Next(-30, 2);
                //    if (particle.flash < 0) {
                //        particle.flash = (int)(particle.flash * MathF.Pow(1f - particle.Scale, 2f));
                //    }
                //}

                if (Projectile.numUpdates == -1) {
                    ExtendPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                }
            }
        }
    }

    public override float SwingProgress(float progress) {
        return SwingProgressAequus(progress);
    }
    public override float GetScale(float progress) {
        float scale = base.GetScale(progress);
        if (progress > 0.1f && progress < 0.9f) {
            return scale + 0.25f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
        }
        return scale;
    }
    public override float GetVisualOuter(float progress, float swingProgress) {
        return 0f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        base.OnHitNPC(target, hit, damageDone);
        //AequusBuff.ApplyBuff<AethersWrath>(target, 360, out bool canPlaySound);
        //if (canPlaySound) {
        //    ModContent.GetInstance<AethersWrathSound>().Play(target.Center);
        //}
        //if (canPlaySound || target.HasBuff<AethersWrath>()) {
        //    for (int i = 0; i < 12; i++) {
        //        var v = Main.rand.NextVector2Unit();
        //        var d = Dust.NewDustPerfect(target.Center + v * new Vector2(Main.rand.NextFloat(target.width / 2f + 16f), Main.rand.NextFloat(target.height / 2f + 16f)), DustID.AncientLight, v * 8f);
        //        d.noGravity = true;
        //        d.noLightEmittence = true;
        //    }
        //}
    }

    public override bool PreDraw(ref Color lightColor) {
        GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out var rotationOffset, out var origin, out var effects);
        var auraColor = GetAuraColor(0f);
        var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
        float size = texture.Size().Length();
        var glowmask = AequusTextures.UltimateSword_Glow.Value;
        float animProgress = AnimProgress;
        float swishProgress = 0f;
        float intensity = 0f;
        if (animProgress > 0.3f && animProgress < 0.65f) {
            swishProgress = (animProgress - 0.3f) / 0.35f;
            intensity = (float)Math.Sin(MathF.Pow(swishProgress, 2f) * MathHelper.Pi);
        }

        for (int i = 0; i < 4; i++) {
            Vector2 v = (i * MathHelper.PiOver2).ToRotationVector2();
            DrawSword(texture, handPosition, frame, GetAuraColor(i * 0.25f) with { A = 0 } * 0.33f * Projectile.Opacity, rotationOffset, origin, effects);
        }

        float trailAlpha = 1f;
        for (float f = lastAnimProgress; f > 0f && f < 1f && trailAlpha > 0f; f += -0.006f) {
            InterpolateSword(f, out var offsetVector, out float _, out float scale, out float outer);

            Main.EntitySpriteDraw(
                glowmask,
                handPosition - Main.screenPosition + offsetVector * GFXOutOffset,
                frame,
                GetAuraColor((1f - trailAlpha) * 0.25f) with { A = 0 } * Projectile.Opacity * 0.6f * trailAlpha * intensity,
                offsetVector.ToRotation() + rotationOffset,
                origin,
                scale * Projectile.scale,
                effects,
                0
            );

            trailAlpha -= 0.04f;
        }

        DrawSword(texture, handPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, rotationOffset, origin, effects);
        DrawSword(glowmask, handPosition, frame, Color.White * Projectile.Opacity, rotationOffset, origin, effects);

        if (intensity > 0f) {
            float progress2 = 1f - (float)Math.Pow(1f - swishProgress, 2f);

            var swish = AequusTextures.SlashForward.Value;
            var swishOrigin = swish.Size() / 2f;
            var swishColor = auraColor with { A = 0 } * MathF.Pow(intensity, 2f) * Projectile.Opacity;
            float r = BaseAngleVector.ToRotation();
            var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
            Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 40f) * baseSwordScale, null, swishColor, r + MathHelper.PiOver2, swishOrigin, new Vector2(1.8f, 2.5f), effects, 0);
        }
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer) {
        base.SendExtraAI(writer);
        writer.Write(Projectile.scale);
    }

    public override void ReceiveExtraAI(BinaryReader reader) {
        base.ReceiveExtraAI(reader);
        Projectile.scale = reader.ReadSingle();
    }
}