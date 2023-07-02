using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Common.Projectiles.Base {
    public abstract class HeldSlashingSwordProjectile : HeldSwordProjectile {
        protected bool _halfWayMark;

        public int swingDirection;

        /// <summary>
        /// Stat which determines how far the sword's laser hitbox will reach, in pixels.
        /// </summary>
        public int swordHeight;
        /// <summary>
        /// Stat which determines how wide the sword's laser hitbox will be, in pixels.
        /// </summary>
        public int swordWidth;
        /// <summary>
        /// Outward offset, only effects rendering.
        /// </summary>
        public int gfxOutOffset;
        /// <summary>
        /// Outward offset, only effects rendering. Updated every game update.
        /// </summary>
        public int animationGFXOutOffset;

        public bool playedSound;

        public int freezeFrame;

        protected float rotationOffset;
        protected float lastAnimProgress;

        private Vector2 angleVector;
        public Vector2 AngleVector { get => angleVector; set => angleVector = Vector2.Normalize(value); }
        public int GFXOutOffset => gfxOutOffset + animationGFXOutOffset;


        public virtual bool ShouldUpdatePlayerDirection() {
            return AnimProgress > 0.4f && AnimProgress < 0.6f;
        }

        public override void SetDefaults() {
            base.SetDefaults();
            swordHeight = 100;
            swordWidth = 30;
        }

        public override bool? CanDamage() {
            return AnimProgress > 0.4f && AnimProgress < 0.6f && freezeFrame <= 0 ? null : false;
        }

        protected override void UpdateSword(Player player, AequusPlayer aequus, float progress) {
            var arm = Main.GetPlayerArmPosition(Projectile);
            if (!_halfWayMark && progress >= 0.5f) {
                if (Projectile.numUpdates != -1 || freezeFrame > 0) {
                    return;
                }
                progress = 0.5f;
                _halfWayMark = true;
            }
            lastAnimProgress = progress;
            InterpolateSword(progress, out var angleVector, out float swingProgress, out float scale, out float outer);
            if (freezeFrame <= 0) {
                AngleVector = angleVector;
            }
            Projectile.position = arm + AngleVector * swordHeight / 2f;
            Projectile.position.X -= Projectile.width / 2f;
            Projectile.position.Y -= Projectile.height / 2f;
            Projectile.rotation = angleVector.ToRotation();
            if (freezeFrame <= 0) {
                UpdateSwing(progress, swingProgress);
            }
            else {
                Projectile.timeLeft++;
            }
            if (Main.netMode != NetmodeID.Server) {
                UpdateArmRotation(player, progress, swingProgress);
                SetArmRotation(player);
            }
            Projectile.scale = scale;
            animationGFXOutOffset = (int)outer;

            if (freezeFrame == 0) {
                swingTime--;
            }
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];
            if (Projectile.numUpdates == -1 && freezeFrame > 0) {
                freezeFrame--;
                if (freezeFrame != 1) {
                    player.itemAnimation++;
                    player.itemTime++;
                }
            }

            if (ShouldUpdatePlayerDirection()) {
                UpdatePlayerDirection(player);
            }

            Projectile.hide = true;
            base.AI();
        }

        protected sealed override void Initialize(Player player, AequusPlayer aequus) {
            AngleVector = Projectile.velocity;

            swingDirection = 1;
            UpdatePlayerDirection(player);
            swingDirection *= Projectile.direction;

            // Flip direction
            if (aequus.itemCombo > 0) {
                swingDirection *= -1;
            }

            InitializeSword(player, aequus);
        }

        protected virtual void InitializeSword(Player player, AequusPlayer aequus) {

        }

        public virtual void UpdateSwing(float progress, float interpolatedSwingProgress) {
        }

        public virtual Vector2 GetOffsetVector(float progress) {
            return BaseAngleVector.RotatedBy((progress * MathHelper.Pi - MathHelper.PiOver2) * -swingDirection);
        }

        public virtual float GetVisualOuter(float progress, float swingProgress) {
            return animationGFXOutOffset;
        }

        public void InterpolateSword(float progress, out Vector2 offsetVector, out float swingProgress, out float scale, out float outer) {
            swingProgress = SwingProgress(progress);
            offsetVector = GetOffsetVector(swingProgress);
            scale = GetScale(swingProgress);
            outer = (int)GetVisualOuter(progress, swingProgress);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            var center = Main.player[Projectile.owner].Center;
            //Helper.DebugDust(center - AngleVector * 20f);
            //Helper.DebugDust(center + AngleVector * (swordHeight * Projectile.scale * baseSwordScale), DustID.CursedTorch);
            return Helper.DeathrayHitbox(center - AngleVector * 20f, center + AngleVector * (swordHeight * Projectile.scale * baseSwordScale), targetHitbox, swordWidth * Projectile.scale * baseSwordScale);
        }

        public void UpdatePlayerDirection(Player player) {
            if (angleVector.X < 0f) {
                //player.direction = -1;
                Projectile.direction = -1;
            }
            else if (angleVector.X > 0f) {
                //player.direction = 1;
                Projectile.direction = 1;
            }
        }

        protected virtual void UpdateArmRotation(Player player, float progress, float swingProgress) {
            var diff = Main.player[Projectile.owner].MountedCenter - Projectile.Center;
            if (Math.Sign(diff.X) == -player.direction) {
                var v = diff;
                v.X = Math.Abs(diff.X);
                armRotation = v.ToRotation();
            }
            else if (progress < 0.1f) {
                if (swingDirection * (progress >= 0.5f ? -1 : 1) * -player.direction == -1) {
                    armRotation = -1.11f;
                }
                else {
                    armRotation = 1.11f;
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write((byte)freezeFrame);
            writer.Write(swingDirection == -1);
            base.SendExtraAI(writer);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            freezeFrame = reader.ReadByte();
            swingDirection = reader.ReadBoolean() ? -1 : 1;
            base.ReceiveExtraAI(reader);
        }

        #region Rendering
        public void DrawDebug() {
            var center = Main.player[Projectile.owner].Center;
            var startOffset = AngleVector * -20f;
            Helper.DrawLine(center - Main.screenPosition + startOffset, center + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, 4f, Color.Green);
            Helper.DrawLine(center + AngleVector.RotatedBy(-MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f - Main.screenPosition, center + AngleVector.RotatedBy(MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f - Main.screenPosition, 4f, Color.Red);
            Helper.DrawLine(center + AngleVector.RotatedBy(-MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, center + AngleVector.RotatedBy(MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, 4f, Color.Red);
            Helper.DrawLine(center + AngleVector.RotatedBy(-MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, center + AngleVector.RotatedBy(-MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f - Main.screenPosition, 4f, Color.Orange);
            Helper.DrawLine(center + AngleVector.RotatedBy(MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, center + AngleVector.RotatedBy(MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f - Main.screenPosition, 4f, Color.Orange);
        }

        public void GetSwordDrawInfo(out Texture2D texture, out Vector2 handPosition, out Rectangle frame, out float rotationOffset, out Vector2 origin, out SpriteEffects effects) {
            Projectile.GetDrawInfo(out texture, out _, out frame, out _, out _);
            handPosition = Main.GetPlayerArmPosition(Projectile) - new Vector2(0f, Main.player[Projectile.owner].gfxOffY);
            rotationOffset = this.rotationOffset;
            if (Main.player[Projectile.owner].direction == swingDirection * -Main.player[Projectile.owner].direction) {
                effects = SpriteEffects.None;
                origin.X = 0f;
                origin.Y = frame.Height;
                rotationOffset += MathHelper.PiOver4;
            }
            else {
                effects = SpriteEffects.FlipHorizontally;
                origin.X = frame.Width;
                origin.Y = frame.Height;
                rotationOffset += MathHelper.PiOver4 * 3f;
            }
        }

        protected void DrawSword(Texture2D texture, Vector2 handPosition, Rectangle frame, Color color, float rotationOffset, Vector2 origin, SpriteEffects effects) {
            Main.EntitySpriteDraw(
                texture,
                handPosition - Main.screenPosition + AngleVector * GFXOutOffset,
                frame,
                color,
                Projectile.rotation + rotationOffset,
                origin,
                Projectile.scale,
                effects,
                0
            );
        }

        protected void DrawSwordAfterImages(Texture2D texture, Vector2 handPosition, Rectangle frame, Color color, float rotationOffset, Vector2 origin, SpriteEffects effects, float loopProgress = 0.07f, float interpolationValue = -0.01f) {
            float trailAlpha = 1f;
            for (float f = lastAnimProgress; f > 0.05f && f < 0.95f && trailAlpha > 0f; f += interpolationValue) {
                InterpolateSword(f, out var offsetVector, out float _, out float scale, out float outer);
                Main.EntitySpriteDraw(
                    texture,
                    handPosition - Main.screenPosition + offsetVector * GFXOutOffset,
                    frame,
                    color * trailAlpha,
                    offsetVector.ToRotation() + rotationOffset,
                    origin,
                    scale,
                    effects,
                    0
                );
                trailAlpha -= loopProgress;
            }
        }

        protected void DrawSwordTipFlare(Vector2 handPosition, float swordTipLength, Vector2 flareSize, Color flareColor, float bloomScale, Color bloomColor) {
            var flare = AequusTextures.Flare.Value;
            var flareOrigin = flare.Size() / 2f;
            var flarePosition = handPosition - Main.screenPosition + AngleVector * swordTipLength * baseSwordScale;
            Main.EntitySpriteDraw(
                AequusTextures.Bloom0,
                flarePosition,
                null,
                bloomColor,
                0f,
                AequusTextures.Bloom0.Size() / 2f,
                bloomScale,
                SpriteEffects.None, 0);
            Main.EntitySpriteDraw(
                flare,
                flarePosition,
                null,
                flareColor,
                0f,
                flareOrigin,
                flareSize,
                SpriteEffects.None, 0);
            Main.EntitySpriteDraw(
                flare,
                flarePosition,
                null,
                flareColor,
                MathHelper.PiOver2,
                flareOrigin,
                flareSize,
                SpriteEffects.None, 0);
        }

        public override void PostDraw(Color lightColor) {
            //DrawDebug();
        }
        #endregion
    }
}