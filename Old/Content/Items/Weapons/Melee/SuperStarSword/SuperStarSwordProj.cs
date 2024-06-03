using Aequus.Common.Projectiles;
using Aequus.Old.Content.StatusEffects;
using System;
using Terraria.Audio;

namespace Aequus.Old.Content.Items.Weapons.Melee.SuperStarSword;

public class SuperStarSwordProj : LegacyHeldSlashingSwordProjectile {
    private bool _spawnedProjectile;

    private static readonly Color[] DustColorsNormal = new Color[] { new Color(40, 160, 255, 0), new Color(10, 255, 255, 0), new Color(100, 180, 255, 0), };
    private static readonly Color[] DustColorsAnniversary = new Color[] { Color.Pink with { A = 0 }, Color.LightPink with { A = 0 }, Color.HotPink with { A = 0 }, };
    public static Color[] DustColors => Main.tenthAnniversaryWorld ? DustColorsAnniversary : DustColorsNormal;

    public override string Texture => AequusTextures.SuperStarSword.Path;

    public override bool ShouldUpdatePlayerDirection() {
        return AnimProgress > 0.1f && AnimProgress < 0.3f;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 66;
        Projectile.height = 66;
        Projectile.noEnchantmentVisuals = true;
        swordHeight = 50;
    }

    public override Color? GetAlpha(Color lightColor) {
        return Color.White;
    }

    public override bool? CanDamage() {
        return AnimProgress > 0.05f && AnimProgress < 0.5f ? null : false;
    }

    public override void AI() {
        base.AI();
        if (!playedSound && AnimProgress > 0.4f) {
            playedSound = true;
            SoundEngine.PlaySound(AequusSounds.HeavySwing with { Pitch = 0.2f }, Projectile.Center);
        }

        if (AnimProgress < 0.3f) {
            var colors = DustColors;
            int amt = !ExtendedMod.HighQualityEffects ? 1 : Main.rand.Next(2) + 1;
            for (int i = 0; i < amt; i++) {
                var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 8f);
                float randomColor = Main.rand.NextFloat(colors.Length);
                Color dustColor = Color.Lerp(colors[(int)randomColor % colors.Length], colors[(int)(randomColor + 1) % colors.Length], randomColor % 1f) with { A = 0 };
                var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), DustID.SilverFlame, velocity, newColor: dustColor);
                d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                d.scale *= Projectile.scale;
                d.fadeIn = d.scale + 0.1f;
                d.noGravity = true;
                if (Projectile.numUpdates == -1) {
                    ExtendPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(10f, 70f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                }
            }
        }
    }

    public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
        if (interpolatedSwingProgress >= 0.5f && !_spawnedProjectile) {
            float lifeRatio = Main.player[Projectile.owner].statLife / (float)Main.player[Projectile.owner].statLifeMax2;
            if (lifeRatio > 0.9f && Main.myPlayer == Projectile.owner) {
                Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_HeldItem(), Main.player[Projectile.owner].Center + BaseAngleVector * 30f,
                    BaseAngleVector * Projectile.velocity.Length() * 9f,
                    ModContent.ProjectileType<SuperStarSwordSlash>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack / 4f, Projectile.owner);
            }
            _spawnedProjectile = true;
        }
    }

    public override Vector2 GetOffsetVector(float progress) {
        return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.25f) - MathHelper.PiOver2 * 1.25f) * -swingDirection);
    }

    public override float SwingProgress(float progress) {
        return SwingProgressStariteSword(progress);
    }

    public override float GetScale(float progress) {
        float scale = base.GetScale(progress);
        if (progress > 0.1f && progress < 0.9f) {
            return scale + 0.5f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
        }
        return scale;
    }

    public override float GetVisualOuter(float progress, float swingProgress) {
        if (progress > 0.6f) {
            float p = 1f - (1f - progress) / 0.4f;
            Projectile.alpha = (int)(p * 255);
            return -8f * p;
        }
        return 0f;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        base.OnHitNPC(target, hit, damageDone);
        target.AddBuff(ModContent.BuffType<BlueFire>(), 480);
    }

    public override bool PreDraw(ref Color lightColor) {
        var center = Main.player[Projectile.owner].Center;
        var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;

        GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out var rotationOffset, out var origin, out var effects);
        DrawSword(texture, handPosition, frame, drawColor, rotationOffset, origin, effects);

        float size = texture.Size().Length();
        if (AnimProgress < 0.4f) {
            float swishProgress = AnimProgress / 0.4f;
            float intensity = (float)Math.Sin((float)Math.Pow(swishProgress, 2f) * MathHelper.Pi);

            var swish = AequusTextures.SlashForward.Value;
            var swishOrigin = swish.Size() / 2f;
            var swishColor = (Main.tenthAnniversaryWorld ? Color.HotPink with { A = 10 } : new Color(10, 30, 100, 10)) * intensity * intensity * Projectile.Opacity;
            float r = BaseAngleVector.ToRotation() + (swishProgress * 2f - 1f) * -swingDirection * 0.4f;
            var swishLocation = Main.player[Projectile.owner].Center - Main.screenPosition;
            Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 40f + 30f * swishProgress) * baseSwordScale, null, swishColor, r + MathHelper.PiOver2, swishOrigin, 1f, effects, 0);
            Main.EntitySpriteDraw(swish, swishLocation + r.ToRotationVector2() * (size - 55f + 30f * swishProgress) * baseSwordScale, null, swishColor * 0.4f, r + MathHelper.PiOver2, swishOrigin, new Vector2(1.2f, 1.75f), effects, 0);
        }
        return false;
    }
}
