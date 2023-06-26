using Aequus;
using Aequus.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Weapons.Melee.Nettlebane;

public class NettlebaneProj : HeldSlashingSwordProjectile {
    public int tier;
    public bool upgradingEffect;
    public bool hitAnything;

    public override void SetStaticDefaults() {
        ProjectileID.Sets.TrailCacheLength[Type] = 70;
        ProjectileID.Sets.TrailingMode[Type] = -1;
        Main.projFrames[Type] = 3;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 40;
        Projectile.height = 40;
        Projectile.extraUpdates = 10;
        Projectile.localNPCHitCooldown *= 10;
        swordHeight = 65;
        swordWidth = 10;
        Projectile.noEnchantmentVisuals = true;
        hitsLeft = 3;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override bool? CanDamage() {
        if (tier == 0) {
            return AnimProgress > 0.05f && AnimProgress < 0.5f ? null : false;
        }
        return base.CanDamage();
    }

    protected override void InitializeSword(Player player, AequusPlayer aequus) {
        tier = 0;
        gfxOutOffset = -6;
        if (player.HasBuff(ModContent.BuffType<NettlebaneBuffTier2>())) {
            tier = 1;
            gfxOutOffset = -12;
        }
        if (player.HasBuff(ModContent.BuffType<NettlebaneBuffTier3>())) {
            tier = 2;
            gfxOutOffset = -24;
        }
        swordHeight += 24 * tier;
        swordWidth += 2 * tier;
        Projectile.width += 45 * tier;
        Projectile.height += 45 * tier;
        swingTimeMax += 3 * tier;
        Projectile.damage = (int)(Projectile.damage * (1f + 0.5f * tier));
    }

    public override void AI() {
        _halfWayMark = true;
        base.AI();
        Projectile.frame = tier;
        if (Main.player[Projectile.owner].itemAnimation <= 1) {
            Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 20 : 0);
        }
        if (!playedSound && AnimProgress > 0.4f) {
            playedSound = true;
            SoundEngine.PlaySound(AequusSounds.swordSwoosh with { Pitch = 0.4f - tier * 0.1f, PitchVariance = 0.1f, }, Projectile.Center);
        }
    }

    public override Vector2 GetOffsetVector(float progress) {
        if (tier <= 1) {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - MathHelper.PiOver2 * 1.5f) * -swingDirection);
        }
        return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - MathHelper.PiOver2 * 1.5f) * -swingDirection * 1.1f);
    }

    public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
        if (progress > 0.85f) {
            Projectile.Opacity = 1f - (progress - 0.85f) / 0.15f;
        }

        if (Projectile.numUpdates <= tier) {

            float min = 0.33f;
            float max = 0.55f;
            if (tier == 0) {
                min = 0f;
                max = 0.2f;
            }

            if (progress > min && progress < max) {
                int amt = 1;
                Color greal = new(0, 200, 0, 128);
                float maxDistance = 55f * Projectile.scale;
                maxDistance *= 1f + tier * 0.5f;
                for (int i = 0; i < amt; i++) {
                    var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, maxDistance / 12f) + BaseAngleVector * Main.rand.NextFloat(5f);
                    var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, maxDistance), DustID.TintableDustLighted, velocity, newColor: greal);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale + Main.rand.NextFloat(maxDistance / 100f);
                    d.fadeIn = d.scale * 0.6f;
                    d.noGravity = true;
                    if (Projectile.numUpdates == -1) {
                        AequusPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                    }
                }
            }
        }

        Projectile.oldPos[0] = AngleVector * 60f * Projectile.scale;
        Projectile.oldRot[0] = Projectile.oldPos[0].ToRotation() + MathHelper.PiOver4;

        // Manually updating oldPos and oldRot 
        for (int i = Projectile.oldPos.Length - 1; i > 0; i--) {
            Projectile.oldPos[i] = Projectile.oldPos[i - 1];
            Projectile.oldRot[i] = Projectile.oldRot[i - 1];
        }
    }

    public override float SwingProgress(float progress) {
        if (tier == 0) {
            return Math.Max((float)Math.Sqrt(Math.Sqrt(Math.Sqrt(SwingProgressAequus(progress)))), MathHelper.Lerp(progress, 1f, progress));
        }
        return SwingProgressSplit(MathF.Pow(progress, 1f));
    }
    public override float GetScale(float progress) {
        float scale = base.GetScale(progress) * 0.77f;
        if (progress > 0.1f && progress < 0.9f) {
            return scale + 0.3f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.6f * MathHelper.Pi), 2f);
        }
        return scale;
    }
    public override float GetVisualOuter(float progress, float swingProgress) {
        return 0f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        base.OnHitNPC(target, hit, damageDone);
        if (hitAnything) {
            return;
        }
        freezeFrame = 4 * tier;
        hitAnything = true;
        var player = Main.player[Projectile.owner];
        if (player.HasBuff<NettlebaneBuffTier3>()) {
            SoundEngine.PlaySound(AequusSounds.largeSlash with { Volume = 0.44f, PitchVariance = 0.1f, }, target.Center);
            player.AddBuff(ModContent.BuffType<NettlebaneBuffTier3>(), 300);
        }
        else if (player.HasBuff<NettlebaneBuffTier2>()) {
            SoundEngine.PlaySound(AequusSounds.swordPowerReady.Sound with { Volume = 0.8f, Pitch = 0.1f, MaxInstances = 2 }, target.Center);
            player.AddBuff(ModContent.BuffType<NettlebaneBuffTier3>(), 300);
            upgradingEffect = true;
        }
        else {
            SoundEngine.PlaySound(AequusSounds.swordPowerReady.Sound with { Volume = 0.8f, MaxInstances = 2 }, target.Center);
            player.AddBuff(ModContent.BuffType<NettlebaneBuffTier2>(), 300);
            upgradingEffect = true;
        }
    }

    public override bool PreDraw(ref Color lightColor) {
        Color glowingColor = new(60, 255, 60, 255);
        var center = Main.player[Projectile.owner].Center;
        var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;

        GetSwordDrawInfo(out var texture, out var handPosition, out var _, out float _, out var _, out var _);
        if (freezeFrame > 0) {
            handPosition += new Vector2(Main.rand.NextFloat(-freezeFrame, freezeFrame), Main.rand.NextFloat(-freezeFrame, freezeFrame));
        }
        var frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
        frame.Width /= 2;
        frame.X = frame.Width * (swingDirection == -1 ? 0 : 1);
        Vector2 origin = new(0f, frame.Height);
        var effects = SpriteEffects.None;
        float size = frame.Size().Length() * (0.4f + tier * 0.12f);
        float rotationOffset = MathHelper.PiOver4;

        for (int i = 0; i < 4; i++) {
            var v = (MathHelper.PiOver2 * i).ToRotationVector2();
            DrawSword(texture, handPosition + v * 2f * Projectile.scale, frame, glowingColor with { A = 0 } * 0.33f * Projectile.Opacity, rotationOffset, origin, effects);
        }

        DrawSword(texture, handPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, rotationOffset, origin, effects);

        float progress = AnimProgress;

        float swishRangeMin = 0.2f;
        float swishRangeMax = 0.8f;
        if (tier == 0) {
            swishRangeMin = 0f;
            swishRangeMax = 0.6f;
        }
        if (progress > swishRangeMin && progress < swishRangeMax) {
            float swishProgress = 1f - MathF.Pow(1f - (progress - swishRangeMin) / (swishRangeMax - swishRangeMin), 2f);
            float intensity = (float)Math.Sin((float)Math.Pow(swishProgress, 2f) * MathHelper.Pi);
            DrawSword(texture, handPosition, frame, new Color(50, 255, 50, 0) * intensity, rotationOffset, origin, effects);

            var swish = AequusTextures.Swish.Value;
            var swishOrigin = swish.Size() / 2f;
            var swishColor = glowingColor.UseA(58) * 0.1f * intensity * intensity * Projectile.Opacity;
            float r = BaseAngleVector.ToRotation() + (swishProgress - 0.5f) * 0.33f * -swingDirection;
            var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
            Main.EntitySpriteDraw(
                swish,
                swishLocation + r.ToRotationVector2() * (size * 0.5f - 16f) * baseSwordScale,
                null,
                swishColor,
                r + MathHelper.PiOver2,
                swishOrigin,
                new Vector2(size / 50f, size / 40f), effects, 0);
        }

        if (upgradingEffect && freezeFrame == 0) {
            var flare = AequusTextures.ShinyFlashParticle.Value;
            var flareOrigin = flare.Size() / 2f;
            float r = AngleVector.ToRotation() + 0.1f * tier * swingDirection;
            float offset = (1.4f - 0.25f * tier) * progress;
            var flareLocation = Main.player[Projectile.owner].Center - Main.screenPosition + r.ToRotationVector2() * size * offset;
            var flareColor = new Color(90, 255, 90, 40) * Projectile.Opacity * 0.33f;
            float flareScale = Projectile.scale * Projectile.Opacity;
            Main.EntitySpriteDraw(
                flare,
                flareLocation,
                null,
                flareColor,
                offset,
                flareOrigin,
                flareScale, effects, 0);
            Main.EntitySpriteDraw(
                flare,
                flareLocation,
                null,
                flareColor,
                MathHelper.PiOver2 + offset,
                flareOrigin,
                flareScale, effects, 0);
            Main.EntitySpriteDraw(
                flare,
                flareLocation,
                null,
                flareColor,
                MathHelper.PiOver2,
                flareOrigin,
                new Vector2(flareScale * 0.5f, flareScale * 2.2f), effects, 0);
        }
        //DrawDebug();
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