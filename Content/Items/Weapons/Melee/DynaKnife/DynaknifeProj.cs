using Aequus.Common.Projectiles;
using Aequus.Content.Graphics.Particles;
using System;
using Terraria.Audio;

namespace Aequus.Content.Items.Weapons.Melee.DynaKnife;

public class DynaknifeProj : LegacyHeldSlashingSwordProjectile {
    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 20;
        Projectile.height = 20;
        Projectile.extraUpdates = 3;
        swordHeight = 44;
        swordWidth = 32;
        gfxOutOffset = -2;
        hitsLeft = 1;
    }

    public override bool? CanDamage() {
        return null;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        base.OnHitNPC(target, hit, damageDone);
        Projectile.NewProjectile(
            Projectile.GetSource_OnHit(target),
            Main.player[Projectile.owner].Center,
            Projectile.velocity,
            ModContent.ProjectileType<DynaknifeStabProj>(),
            Projectile.damage,
            Projectile.knockBack,
            Projectile.owner,
            target.whoAmI
        );
        var player = Main.player[Projectile.owner];
        player.immuneTime = Math.Max(player.immuneTime, 12);
        player.immuneNoBlink = true;
        player.velocity.X = -player.direction * 8f;
        player.velocity.Y = Math.Min(player.velocity.Y, -3f);
        SoundEngine.PlaySound(AequusSounds.InflictBlood, Projectile.Center);
    }

    protected override void InitializeSword(Player player, AequusPlayer aequus) {
        swingDirection = -player.direction;
        rotationOffset = -MathHelper.PiOver4 * swingDirection;
    }

    public override void AI() {
        base.AI();

        if (freezeFrame > 0) {
            return;
        }

        if (!playedSound) {
            playedSound = true;
            SoundEngine.PlaySound(AequusSounds.UseDagger with { Volume = 0.4f }, Projectile.Center);
        }

        if (Main.netMode != NetmodeID.Server) {
            Rectangle hitbox = Main.player[Projectile.owner].getRect();
            if (Cull2D.Rectangle(hitbox) && Main.player[Projectile.owner].velocity.Length() > 6f && Main.player[Projectile.owner].altFunctionUse == 2 && (Main.player[Projectile.owner].itemTimeMax - Main.player[Projectile.owner].itemTime + 1) % 10 == 0) {
                var particle = DashParticles.New();
                particle.Location = Main.rand.NextVector2FromRectangle(hitbox) - Main.player[Projectile.owner].velocity;
                particle.Velocity = new Vector2(Main.player[Projectile.owner].velocity.X * 0.8f, Main.player[Projectile.owner].velocity.Y * 0.4f);
                particle.Rotation = particle.Velocity.ToRotation() + MathHelper.Pi;
                particle.Scale = Main.rand.NextFloat(1f, 1.5f);
            }
        }
    }

    public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
    }

    public override Vector2 GetOffsetVector(float progress) {
        return base.GetOffsetVector(progress).RotatedBy(swingDirection * 0.8f);
    }

    public override float SwingProgress(float progress) {
        return 1f - MathF.Pow(1f - progress, 3f);
    }

    public override float GetScale(float progress) {
        return base.GetScale(progress);
    }

    public override float GetVisualOuter(float progress, float swingProgress) {
        return 0f;
    }

    public override bool PreDraw(ref Color lightColor) {
        var glowColor = new Color(120, 20, 36, 66);
        var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
        float animProgress = AnimProgress;
        //float swishProgress = 0f;
        //float intensity = 0f;
        //if (animProgress > 0f) {
        //    swishProgress = 1f - MathF.Pow(1f - animProgress, 5f);
        //    intensity = (float)Math.Sin(MathF.Pow(swishProgress, 2f) * MathHelper.Pi);
        //}

        GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out float rotOffset, out var origin, out var effects);
        origin.X = Math.Clamp(origin.X, 4f, frame.Width - 4f);
        DrawSword(texture, handPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, rotOffset, origin, effects);
        DrawSword(AequusTextures.DynaknifeProj_Glow, handPosition, frame, Color.White with { A = 0 } * Projectile.Opacity, rotOffset, origin, effects);

        //if (intensity > 0f) {
        //    var swish = AequusTextures.Swish.Value;
        //    var swishOrigin = swish.Size() / 2f;
        //    float r = BaseAngleVector.ToRotation();
        //    r += (MathF.Sin(animProgress * MathHelper.PiOver2) - 0.5f - 0.05f) * -swingDirection * 5f;
        //    var swishLocation = Main.player[Projectile.owner].Center - new Vector2(0f, Main.player[Projectile.owner].gfxOffY) - Main.screenPosition;

        //    Main.EntitySpriteDraw(
        //        swish,
        //        swishLocation + r.ToRotationVector2() * 24f * Projectile.scale,
        //        null,
        //        glowColor * MathF.Pow(intensity, 3f),
        //        r + MathHelper.PiOver2,
        //        swishOrigin,
        //        new Vector2(0.5f, 0.6f),
        //        effects,
        //        0
        //    );
        //}
        return false;
    }
}
