﻿using Aequus.Common.Projectiles;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Content.Dedicated.IronLotus;

public class IronLotusProj : LegacyHeldSlashingSwordProjectile {
    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 400;
        Projectile.height = 400;
        Projectile.localNPCHitCooldown = 50;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.noEnchantmentVisuals = true;
        Projectile.ownerHitCheck = false;
        swordHeight = 132;
        rotationOffset = MathHelper.PiOver4;
        hitsLeft = 5;
    }

    public override void AI() {
        base.AI();
        //if (Main.player[Projectile.owner].itemAnimation <= 1) {
        //    Main.player[Projectile.owner].Aequus().itemCombo = (ushort)(combo == 0 ? 20 : 0);
        //}
    }

    public override float GetVisualOuter(float progress, float swingProgress) {
        return -170f + ((float)Math.Sin(swingProgress * MathHelper.Pi) + 1f) * 80f;
    }

    public override float GetScale(float progress) {
        return base.GetScale(progress) + (float)Math.Sin(progress * MathHelper.Pi) * 0.4f;
    }

    public override float SwingProgress(float progress) {
        if (progress >= 0.5f)
            return progress;
        return SwingProgressAequus(progress);
    }

    public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
        if (AnimProgress > 0.4f && AnimProgress < 0.6f) {
            if (!playedSound) {
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot with { Pitch = 0.5f }, Projectile.Center);
                playedSound = true;
            }
            float particleDistance = (swordHeight + 40f) * Projectile.scale;
            float particleRandomRotation = 0.3f;
            for (int i = 0; i < 15; i++) {
                ExtendPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector.RotatedBy(Main.rand.NextFloat(-particleRandomRotation, particleRandomRotation)) * Main.rand.NextFloat(particleDistance), AngleVector, Main.player[Projectile.owner], showMagmaStone: false);
                var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector.RotatedBy(Main.rand.NextFloat(-particleRandomRotation, particleRandomRotation)) * Main.rand.NextFloat(particleDistance), DustID.Torch, Scale: Main.rand.NextFloat(0.5f, 2f));
                d.velocity += AngleVector;
                d.velocity *= 4f;
                d.noGravity = true;
                d.fadeIn = d.scale + 0.1f;
            }
            for (int i = 0; i < 2; i++) {
                var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * (Main.rand.Next(150, 250) + animationGFXOutOffset), DustID.Torch, Scale: Main.rand.NextFloat(1f, 3f));
                d.velocity = -AngleVector * 3f;
                d.noGravity = true;
                d.fadeIn = d.scale + 0.33f;
            }
        }
    }

    public override Vector2 GetOffsetVector(float progress) {
        return BaseAngleVector.RotatedBy(Math.Sin(progress * MathHelper.TwoPi * swingDirection) * 0.2f);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        float _ = float.NaN;
        var end = Projectile.Center + BaseAngleVector * Projectile.width / 2f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Main.player[Projectile.owner].Center, end, 70f * Projectile.scale, ref _);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(ModContent.BuffType<IronLotusDebuff>(), 240);
        Projectile.NewProjectile(Projectile.GetSource_OnHit(target), Main.rand.NextVector2FromRectangle(target.getRect()), AngleVector * 0.1f, ModContent.ProjectileType<IronLotusFlare>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
    }

    protected override void SetArmRotation(Player player) {
    }

    public override bool PreDraw(ref Color lightColor) {
        var texture = TextureAssets.Projectile[Type].Value;
        var center = Main.player[Projectile.owner].Center;
        var handPosition = Main.GetPlayerArmPosition(Projectile) + AngleVector * animationGFXOutOffset;
        var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
        var drawCoords = handPosition - Main.screenPosition;
        float size = texture.Size().Length();
        var effects = SpriteEffects.None;
        var origin = new Vector2(0f, texture.Height);
        float rotation = Projectile.rotation + rotationOffset;
        if (BaseAngleVector.X < 0f) {
            effects = SpriteEffects.FlipHorizontally;
            rotation += MathHelper.PiOver2;
            origin.X = texture.Width;
        }

        Main.EntitySpriteDraw(texture, handPosition - Main.screenPosition, null, drawColor, rotation, origin, Projectile.scale, effects, 0);

        float wave = MathF.Pow(MathF.Sin(AnimProgress * MathHelper.Pi), 2f);
        var edges = AequusTextures.IronLotusProj_Edges.Value;
        var edgesColor = new Color(100, 60, 5, 0) * Projectile.Opacity * wave;
        for (int i = 0; i < 4; i++) {
            Main.EntitySpriteDraw(edges, drawCoords + (i * MathHelper.PiOver2 + Main.GlobalTimeWrappedHourly * 10f).ToRotationVector2() * 2f * Projectile.scale, null, edgesColor, rotation, origin, Projectile.scale, effects, 0);
        }
        Main.EntitySpriteDraw(AequusTextures.IronLotusProj_EdgesAura, drawCoords, null, Color.Red with { A = 0 } * Projectile.Opacity * wave, rotation, origin, Projectile.scale, effects, 0);

        float swingProgress = SwingProgress(AnimProgress);
        if (swingProgress > 0.1f && swingProgress < 0.9f) {
            float progress = (swingProgress - 0.1f) / 0.8f;
            float intensity = (float)Math.Sin(progress * MathHelper.Pi);
            var shineColor = new Color(200, 120, 40, 0) * intensity * intensity * Projectile.Opacity;
            Texture2D slash = AequusTextures.SlashForward.Value;
            var slashLocation = Main.GetPlayerArmPosition(Projectile) + BaseAngleVector * (60f + progress * 150f * Projectile.scale) - Main.screenPosition;
            Main.EntitySpriteDraw(slash, slashLocation, null, shineColor, BaseAngleVector.ToRotation() + MathHelper.PiOver2, slash.Size() / 2f, new Vector2(Projectile.scale * 0.8f, Projectile.scale * 2f), effects, 0);
        }
        return false;
    }
}