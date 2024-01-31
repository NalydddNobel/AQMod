using Aequus.Common.Projectiles;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.Weapons.Demon.Melee;

public class CauterizerProj : LegacyHeldSlashingSwordProjectile {
    public override string Texture => AequusTextures.Cauterizer.Path;

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 145;
        Projectile.height = 145;
        swordHeight = 90;
        gfxOutOffset = -6;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override void AI() {
        base.AI();
        if (!playedSound && AnimProgress > 0.4f) {
            playedSound = true;
            SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(-1f), Projectile.Center);
        }
        if (AnimProgress > 0.3f && AnimProgress < 0.6f) {
            int amt = (int)Math.Max((Main.rand.Next(4) + 1) * Main.gfxQuality, 1f);
            for (int i = 0; i < amt; i++) {
                var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 8f);
                var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, velocity, newColor: Color.Orange with { A = 0 });
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                d.scale *= Projectile.scale;
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
                if (Projectile.numUpdates == -1) {
                    PlayerHelper.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                }
            }
        }
    }

    public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
        if (progress == 0.5f && Main.myPlayer == Projectile.owner) {
            Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_HeldItem(), Projectile.Center,
                AngleVector * Projectile.velocity.Length() * 9f,
                ModContent.ProjectileType<CauterizerSlash>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack / 4f, Projectile.owner);
        }
    }

    public override Vector2 GetOffsetVector(float progress) {
        return progress < 0.5f
            ? base.GetOffsetVector(progress)
            : BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - MathHelper.PiOver2 * 1.5f) * -swingDirection * (0.8f + 0.2f * Math.Min(Main.player[Projectile.owner].GetModPlayer<AequusPlayer>().itemUsage / 300f, 1f)));
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
        freezeFrame = 4;
        target.AddBuff(ModContent.BuffType<CrimsonHellfire>(), 240);
    }

    public override bool PreDraw(ref Color lightColor) {
        var center = Main.player[Projectile.owner].Center;
        var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
        var glowColor = new Color(255, 155, 80, 0);
        float animProgress = AnimProgress;
        float swishProgress = 0f;
        float intensity = 0f;
        if (animProgress > 0.3f && animProgress < 0.65f) {
            swishProgress = (animProgress - 0.3f) / 0.35f;
            intensity = (float)Math.Sin(MathF.Pow(swishProgress, 2f) * MathHelper.Pi);
        }

        GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out float rotationOffset, out var origin, out var effects);
        float size = texture.Size().Length();
        if (Aequus.HighQualityEffects) {
            DrawSwordAfterImages(texture, handPosition, frame, glowColor * 0.4f * Projectile.Opacity, rotationOffset, origin, effects,
                loopProgress: 0.17f, interpolationValue: -0.01f);

            float auraOffsetMagnitude = (2f + intensity * 4f) * Projectile.scale * baseSwordScale;
            for (float i = 0; i < MathHelper.TwoPi; i += MathHelper.PiOver2) {
                DrawSword(texture, handPosition + i.ToRotationVector2() * auraOffsetMagnitude, frame, glowColor * 0.33f * Projectile.Opacity, rotationOffset, origin, effects);
            }
        }
        DrawSword(texture, handPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, rotationOffset, origin, effects);

        if (intensity > 0f) {
            var shine = AequusTextures.Flare;
            var shineOrigin = shine.Size() / 2f;
            var shineColor = new Color(200, 120, 40, 100) * intensity * intensity * Projectile.Opacity;
            var shineLocation = handPosition - Main.screenPosition + AngleVector * (size - 8f) * Projectile.scale;
            Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, 0f, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale) * intensity, effects, 0);
            Main.EntitySpriteDraw(shine, shineLocation, null, shineColor, MathHelper.PiOver2, shineOrigin, new Vector2(Projectile.scale * 0.5f, Projectile.scale * 2f) * intensity, effects, 0);
        }
        return false;
    }
}