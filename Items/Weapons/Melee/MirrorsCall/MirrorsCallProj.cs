using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Aequus.Common.Projectiles.Base;

namespace Aequus.Items.Weapons.Melee.MirrorsCall {
    public class MirrorsCallProj : HeldSwordProjectile {
        public override string Texture => AequusTextures.MirrorsCall.Path;

        public override int AmountAllowedActive => -1;

        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.ownerHitCheck = true;
        }

        protected override void Initialize(Player player, AequusPlayer aequus) {
            var npc = Projectile.FindTargetWithinRange(300f);
            if (npc != null) {
                BaseAngleVector = Projectile.DirectionTo(npc);
            }
            float scaleIncrease = 1f + Main.rand.NextFloat(-0.4f, 0.1f);
            Projectile.Resize((int)(Projectile.width * scaleIncrease), (int)(Projectile.height * scaleIncrease));
            Projectile.scale += (scaleIncrease - 1f);

            Projectile.ai[0] = Main.rand.NextFloat(100f, 190f);
            Projectile.ai[1] = Main.rand.NextBool() ? 1 : -1;
            BaseAngleVector = BaseAngleVector.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
            swingTimeMax *= 6;
        }

        protected override void UpdateSword(Player player, AequusPlayer aequus, float progress) {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = player.Center + BaseAngleVector * Projectile.ai[0];
            swingTime--;
            player.itemTime = Math.Min(player.itemTime, player.itemTimeMax);
            if (Projectile.localAI[0] == 0) {
                Projectile.localAI[0] = Main.rand.NextFloat(0.1f, 6f);
            }
            if (Projectile.localAI[1] == 0 && swingTime * 2 < swingTimeMax) {
                Projectile.localAI[1] = 1f;
                SoundEngine.PlaySound(AequusSounds.swordSwoosh.Sound with { Pitch = 0.7f, PitchVariance = 0.3f, MaxInstances = 20, Volume = 0.4f }, Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Main.instance.LoadProjectile(ProjectileID.NightsEdge);
            float progress = SwingProgressAequus(AnimProgress);
            var texture = TextureAssets.Projectile[ProjectileID.NightsEdge].Value;
            var swordPosition = Projectile.Center - Main.screenPosition;
            float opacity = MathF.Sin(progress * MathHelper.Pi);
            var frame = texture.Frame(verticalFrames: 4, frameY: 0);
            var origin = frame.Size() / 2f;
            var rainbowColor = Helper.GetRainbowColor(Main.LocalPlayer, Projectile.localAI[0] + progress * 2f) with { A = 0 };
            var scale = new Vector2(1f, 1.5f) * Projectile.scale;
            var swordTexture = AequusTextures.MirrorsCall_Aura;
            var swordOrigin = new Vector2(0f, swordTexture.Height);
            float dir = Projectile.direction * Projectile.ai[1];
            float slashRotation = Projectile.rotation + (-2f + progress * 4f) * dir;
            float swingOutwards = 60f + 70f * opacity;
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
                for (int i = 1; i < 7; i++) {
                    float interpolatedProgress = progress - 1f / swingTimeMax * i;
                    float interpolatedSlashRotation = Projectile.rotation + (-2f + interpolatedProgress * 4f) * dir;
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

            for (float f = 0f; f < 1f; f += 0.125f) {
                var spinningPoint = (f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 5f).ToRotationVector2();
                Main.EntitySpriteDraw(
                    texture,
                    slashPosition + spinningPoint * 2f * Projectile.scale,
                    frame,
                    Helper.GetRainbowColor(Main.LocalPlayer, f * 6f) with { A = 0 } * opacity,
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
}