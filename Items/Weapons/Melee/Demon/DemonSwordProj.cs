using Aequus;
using Aequus.Buffs.Debuffs;
using Aequus.Common.Projectiles.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Aequus.Items.Weapons.Melee.Demon.Cauterizer {
    public class DemonSwordProj : HeldSlashingSwordProjectile {
        public override string Texture => AequusTextures.DemonSword.Path;

        public override void SetDefaults() {
            base.SetDefaults();
            Projectile.width = 205;
            Projectile.height = 205;
            swordHeight = 160;
            gfxOutOffset = -6;
            hitsLeft = 60;
            Projectile.extraUpdates = 9;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void AI() {
            base.AI();
            if (!playedSound && AnimProgress > 0.2f) {
                playedSound = true;
                SoundEngine.PlaySound(AequusSounds.swordSwoosh with { Pitch = -0.7f, PitchVariance = 0.1f,}, Projectile.Center);
            }
            if (AnimProgress > 0.3f && AnimProgress < 0.6f && freezeFrame <= 0 && Projectile.numUpdates == -1) {
                int amt = !Aequus.HQ ? 1 : 4;
                for (int i = 0; i < amt; i++) {
                    var velocity = AngleVector.RotatedBy(MathHelper.PiOver2 * -swingDirection) * Main.rand.NextFloat(2f, 8f);
                    var d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(40f, 140f * Projectile.scale), DustID.Paint, velocity, newColor: new Color(100, 100, 100, 255), Scale: 1.7f);
                    d.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
                    d.scale *= Projectile.scale;
                    d.fadeIn = d.scale + 0.2f;
                    d.noGravity = true;
                    if (Projectile.numUpdates == -1) {
                        AequusPlayer.SpawnEnchantmentDusts(Main.player[Projectile.owner].Center + AngleVector * Main.rand.NextFloat(40f, 120f * Projectile.scale), velocity, Main.player[Projectile.owner]);
                    }
                }
            }
        }

        public override void UpdateSwing(float progress, float interpolatedSwingProgress) {
            //if (progress == 0.5f && Main.myPlayer == Projectile.owner) {
            //    Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_HeldItem(), Projectile.Center,
            //        AngleVector * Projectile.velocity.Length() * 9f,
            //        ModContent.ProjectileType<CauterizerSlash>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack / 4f, Projectile.owner);
            //}
        }

        public override Vector2 GetOffsetVector(float progress) {
            return BaseAngleVector.RotatedBy((progress * (MathHelper.Pi * 1.5f) - MathHelper.PiOver2 * 1.5f) * -swingDirection);
        }

        public override float SwingProgress(float progress) {
            return SwingProgressSplit(progress);
        }

        public override float GetScale(float progress) {
            float scale = base.GetScale(progress);
            if (progress > 0.1f && progress < 0.9f) {
                return scale + 0.3f * (float)Math.Pow(Math.Sin((progress - 0.1f) / 0.9f * MathHelper.Pi), 2f);
            }
            return scale;
        }

        public override float GetVisualOuter(float progress, float swingProgress) {
            return 0f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuffs(240, 1, CorruptionHellfire.Debuffs);
            target.AddBuffs(240, 1, CrimsonHellfire.Debuffs);
        }

        private void DrawBlackHoleSword(Texture2D swordTexture, Color slashColor, Color swordColor, Vector2 handPosition, float size, Rectangle swordFrame, float swordRotation, Vector2 swordOrigin, SpriteEffects swordEffects, Texture2D swish, Rectangle swishFrame, float intensity) {
            if (intensity > 0f) {
                var swishLocation = handPosition - Main.screenPosition + AngleVector * ((size - 120f) * Projectile.scale);
                int dir = swingDirection;
                float rotation = AngleVector.ToRotation();
                if (dir == -1) {
                    rotation += MathHelper.Pi;
                }
                Main.spriteBatch.Draw(swish, swishLocation, swishFrame, slashColor * intensity, rotation, swishFrame.Size() / 2f, Projectile.scale * 1.5f, dir.ToSpriteEffect(), 0f);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            var swish = AequusTextures.Swish3;
            var swishFrame = swish.Frame(verticalFrames: 4, frameY: 0);

            var center = Main.player[Projectile.owner].Center;
            var drawColor = Projectile.GetAlpha(lightColor) * Projectile.Opacity;
            var glowColor = Color.Lerp(Color.Red, Color.BlueViolet, Helper.Wave(Main.GlobalTimeWrappedHourly * 10f, 0f, 1f));
            float animProgress = AnimProgress;
            float intensity = 0f;
            if (animProgress > 0.3f && animProgress < 0.65f) {
                intensity = (float)Math.Sin(MathF.Pow((animProgress - 0.3f) / 0.35f, 2f) * MathHelper.Pi);
            }

            GetSwordDrawInfo(out var texture, out var handPosition, out var frame, out float rotationOffset, out var origin, out var effects);
            handPosition += Main.rand.NextVector2Square(0f, freezeFrame * 10f);
            float size = texture.Size().Length();

            DrawSword(texture, handPosition, frame, Projectile.GetAlpha(lightColor) * Projectile.Opacity, rotationOffset, origin, effects);
            if (Aequus.HQ) {
                for (float f = 0; f < MathHelper.TwoPi; f += MathHelper.PiOver2) {
                    DrawBlackHoleSword(AequusTextures.DemonSword_PortalOutline, glowColor, glowColor, handPosition + f.ToRotationVector2() * 2f, size, frame, rotationOffset, origin, effects, swish, swishFrame, MathF.Pow(intensity, 2f));
                }
            }
            DrawSword(AequusTextures.DemonSword_PortalOutline, handPosition, frame, glowColor, rotationOffset, origin, effects);
            DrawBlackHoleSword(AequusTextures.DemonSword_Void, Color.Black, Color.White, handPosition, size, frame, rotationOffset, origin, effects, swish, swishFrame, intensity);
            DrawSword(AequusTextures.DemonSword_Void, handPosition, frame, Color.White, rotationOffset, origin, effects);
            return false;
        }
    }
}