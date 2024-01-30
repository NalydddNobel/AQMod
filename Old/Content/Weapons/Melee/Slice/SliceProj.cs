using Aequus.Common.Projectiles;
using System;
using System.IO;
using Terraria.Audio;

namespace Aequus.Old.Content.Weapons.Melee.Slice;

public class SliceProj : LegacyHeldSlashingSwordProjectile {
    public override String Texture => AequusTextures.Slice.Path;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 120;
        Projectile.height = 120;
        Projectile.extraUpdates = 6;
        swordHeight = 105;
        swordWidth = 20;
        gfxOutOffset = -12;
        hitsLeft = 3;
    }

    public override Color? GetAlpha(Color lightColor) {
        return lightColor.MaxRGBA(222);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, Int32 damageDone) {
        base.OnHitNPC(target, hit, damageDone);
        SliceOnHitEffect.SpawnOnNPC(Projectile, target);
        target.AddBuff(BuffID.Frostburn2, Slice.SwordDebuffDuration);
        //freezeFrame = Math.Max(8 - TimesSwinged / 2, 0);
    }

    public override void OnHitPlayer(Player target, Player.HurtInfo info) {
        target.AddBuff(BuffID.Frostburn, Slice.SwordDebuffDuration);
    }

    protected override void InitializeSword(Player player, AequusPlayer aequus) {
        swingTimeMax = Math.Max(swingTimeMax - Math.Clamp(TimesSwinged, 0, 10), 10);
    }

    public override void AI() {
        base.AI();

        Single progress = AnimProgress;
        if (Projectile.numUpdates == -1 && progress > 0.33f && progress < 0.55f) {
            for (Int32 i = 0; i < 3; i++) {
                var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 12f);
                var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, velocity, newColor: new Color(80, 155, 255, 128), Scale: 2f);
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                d.scale *= Projectile.scale * 0.6f;
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
                if (i == 0) {
                    PlayerHelper.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                }
            }
        }

        if (freezeFrame > 0) {
            return;
        }

        if (!playedSound && AnimProgress > 0.4f) {
            playedSound = true;
            SoundEngine.PlaySound(AequusSounds.HeavySwing with { Volume = 0.7f, }, Projectile.Center);
        }
    }

    public override void UpdateSwing(Single progress, Single interpolatedSwingProgress) {
        if (Projectile.numUpdates != -1) {
            return;
        }

        var player = Main.player[Projectile.owner];
        if (progress == 0.5f && Main.myPlayer == Projectile.owner && player.altFunctionUse != 2) {
            Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_HeldItem(), Projectile.Center,
                AngleVector * Projectile.velocity.Length() * 3f,
                ModContent.ProjectileType<SliceBulletProj>(), (Int32)(Projectile.damage * 0.5f), Projectile.knockBack / 4f, Projectile.owner);
        }
    }

    public override Vector2 GetOffsetVector(Single progress) {
        return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.75f) - MathHelper.PiOver2 * 1.75f) * -swingDirection * (0.9f + 0.1f * Math.Min(TimesSwinged / 5f, 1f)));
    }

    public override Single SwingProgress(Single progress) {
        return SwingProgressSplit(progress);
    }

    public override Single GetScale(Single progress) {
        Single scale = base.GetScale(progress);
        if (progress > 0.1f && progress < 0.9f) {
            return scale + 0.5f * (Single)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
        }
        return scale;
    }

    public override Single GetVisualOuter(Single progress, Single swingProgress) {
        return 0f;
    }

    public override Boolean PreDraw(ref Color lightColor) {
        var glowColor = new Color(80, 155, 255, 0);
        var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
        Single animProgress = AnimProgress;
        Single swishProgress = 0f;
        Single intensity = 0f;
        if (animProgress > 0.3f && animProgress < 0.65f) {
            swishProgress = (animProgress - 0.3f) / 0.35f;
            intensity = (Single)Math.Sin(MathF.Pow(swishProgress, 2f) * MathHelper.Pi);
        }

        GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out Single rotationOffset, out var origin, out var effects);
        if (Aequus.highQualityEffects) {
            DrawSwordAfterImages(texture, handPosition, frame, glowColor * 0.4f * Projectile.Opacity, rotationOffset, origin, effects,
                loopProgress: 0.07f, interpolationValue: -0.01f);

            Single auraOffsetMagnitude = (2f + intensity * 4f) * Projectile.scale * baseSwordScale;
            for (Single i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2) {
                DrawSword(texture, handPosition + i.ToRotationVector2() * auraOffsetMagnitude, frame, glowColor * 0.33f * Projectile.Opacity, rotationOffset, origin, effects);
            }
        }

        Single swishIntensity = MathF.Sin(animProgress * MathHelper.Pi);
        var swish = AequusTextures.SlashVanilla.Value;
        var swishFrame = swish.Frame(verticalFrames: 4, frameY: 0);
        var swishOrigin = swishFrame.Size() / 2f;
        var swishColor = glowColor with { A = 58 } * 0.7f * MathF.Pow(intensity, 1.5f) * Projectile.Opacity;
        var swishLocation = Main.player[Projectile.owner].MountedCenter - Main.screenPosition;
        var swishEffects = swingDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
        Main.EntitySpriteDraw(
            swish,
            swishLocation + AngleVector * 10f * Projectile.scale,
            swishFrame, swishColor, AngleVector.ToRotation() + swingDirection * 0.4f, swishOrigin, 1.3f + intensity * 0.6f, swishEffects, 0);

        DrawSword(texture, handPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, rotationOffset, origin, effects);

        if (animProgress < 0.6f) {
            Single flareIntensity = animProgress / 0.6f;
            var flareColor = glowColor with { A = 0 } * flareIntensity * swishProgress;
            DrawSwordTipFlare(handPosition, (swishFrame.Height + 50f) * Projectile.scale / 2f, new Vector2(0.9f, 2f) * Helper.Oscillate(Main.GlobalTimeWrappedHourly * 40f, 0.8f, 1f) * flareIntensity, flareColor, 0.7f * flareIntensity, flareColor.HueAdd(0.07f) with { A = 0 });
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