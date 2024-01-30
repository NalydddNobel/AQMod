using Aequus.Common.Projectiles;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace Aequus.Content.DedicatedContent.MirrorsCall;

public class MirrorsCallProj : LegacyHeldSwordProjectile {
    public override String Texture => ModContent.GetInstance<MirrorsCall>().Texture;

    public override Int32 AmountAllowedActive => -1;

    public override void SetDefaults() {
        base.SetDefaults();
        Projectile.width = 400;
        Projectile.height = 400;
        Projectile.ownerHitCheck = true;
        hitsLeft = Int32.MaxValue;
    }

    protected override void Initialize(Player player, AequusPlayer aequus) {
        var npc = Projectile.FindTargetWithinRange(300f);
        if (npc != null) {
            BaseAngleVector = Projectile.DirectionTo(npc.Center);
        }
        Single scaleIncrease = 1f + Main.rand.NextFloat(-0.4f, 0.1f);
        Projectile.Resize((Int32)(Projectile.width * scaleIncrease), (Int32)(Projectile.height * scaleIncrease));
        Projectile.scale += scaleIncrease - 1f;

        Projectile.ai[0] = Main.rand.NextFloat(100f, 190f);
        Projectile.ai[1] = Main.rand.NextBool() ? 1 : -1;
        BaseAngleVector = BaseAngleVector.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
        swingTimeMax *= 6;
    }

    protected override void UpdateSword(Player player, AequusPlayer aequus, Single progress) {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.Center = player.Center + BaseAngleVector * Projectile.ai[0];
        swingTime--;
        player.itemTime = Math.Min(player.itemTime, player.itemTimeMax);
        if (Projectile.localAI[0] == 0) {
            Projectile.localAI[0] = Main.rand.NextFloat(0.1f, 6f);
        }
        if (Projectile.localAI[1] == 0 && swingTime * 2 < swingTimeMax) {
            Projectile.localAI[1] = 1f;
            SoundEngine.PlaySound(AequusSounds.HeavySwing with { Pitch = 0.7f, PitchVariance = 0.3f, MaxInstances = 20, Volume = 0.4f }, Projectile.Center);
        }
    }

    public override Boolean PreDraw(ref Color lightColor) {
        Main.instance.LoadProjectile(ProjectileID.NightsEdge);
        Single progress = SwingProgressAequus(AnimProgress);
        var texture = TextureAssets.Projectile[ProjectileID.NightsEdge].Value;
        var swordPosition = Projectile.Center - Main.screenPosition;
        Single opacity = MathF.Sin(progress * MathHelper.Pi);
        var frame = texture.Frame(verticalFrames: 4, frameY: 0);
        var origin = frame.Size() / 2f;
        var rainbowColor = ExtendColor.GetLastPrismColor(Main.LocalPlayer, Projectile.localAI[0] + progress * 2f) with { A = 0 };
        var scale = new Vector2(1f, 1.5f) * Projectile.scale;
        var swordTexture = AequusTextures.MirrorsCall_Aura;
        var swordOrigin = new Vector2(0f, swordTexture.Height());
        Single dir = Projectile.direction * Projectile.ai[1];
        Single slashRotation = Projectile.rotation + (-2f + progress * 4f) * dir;
        Single swingOutwards = 60f + 70f * opacity;
        var slashPosition = swordPosition + slashRotation.ToRotationVector2() * swingOutwards * Projectile.scale;

        Main.EntitySpriteDraw(
            swordTexture,
            slashPosition - slashRotation.ToRotationVector2() * 100f / Projectile.scale,
            null,
            Color.White with { A = 0 } * opacity,
            slashRotation + MathHelper.PiOver4,
            swordOrigin,
            1f,
            SpriteEffects.None,
            0f
        );

        if (swingTime < swingTimeMax - 1) {
            for (Int32 i = 1; i < 7; i++) {
                Single interpolatedProgress = progress - 1f / swingTimeMax * i;
                Single interpolatedSlashRotation = Projectile.rotation + (-2f + interpolatedProgress * 4f) * dir;
                var interpolatedSlashPosition = swordPosition + interpolatedSlashRotation.ToRotationVector2() * swingOutwards * Projectile.scale;

                Main.EntitySpriteDraw(
                    texture,
                    interpolatedSlashPosition,
                    frame,
                    rainbowColor.HueAdd(-0.04f * i) * opacity * ((7 - i) / 4f),
                    interpolatedSlashRotation,
                    origin,
                    scale * 1.1f,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        for (Single f = 0f; f < 1f; f += 0.125f) {
            var spinningPoint = (f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 5f).ToRotationVector2();
            Main.EntitySpriteDraw(
                texture,
                slashPosition + spinningPoint * 2f * Projectile.scale,
                frame,
                ExtendColor.GetLastPrismColor(Main.LocalPlayer, f * 6f) with { A = 0 } * opacity,
                slashRotation,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
        return false;
    }
}