using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles.Base {
    public abstract class SwordProjectileBase : ModProjectile {
        private bool _init;
        protected bool _halfWayMark;

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

        public int swingTime;
        public int swingTimeMax;
        public int swingDirection;

        public bool playedSound;

        public int combo;

        public int freezeFrame;

        protected float rotationOffset;
        protected float baseSwordScale;
        protected float lastAnimProgress;

        private float armRotation;
        private Vector2 angleVector;
        public Vector2 AngleVector { get => angleVector; set => angleVector = Vector2.Normalize(value); }
        public Vector2 BaseAngleVector { get => Vector2.Normalize(Projectile.velocity); set => Projectile.velocity = Vector2.Normalize(value); }
        public virtual float AnimProgress => Math.Clamp(1f - swingTime / (float)swingTimeMax, 0f, 1f);
        public int GFXOutOffset => gfxOutOffset + animationGFXOutOffset;

        public int amountAllowedToHit;

        public int TimesSwinged {
            get {
                return Main.player[Projectile.owner].Aequus().itemUsage / 60;
            }
            set {
                Main.player[Projectile.owner].Aequus().itemUsage = (ushort)(value * 60);
            }
        }

        public virtual bool ShouldUpdatePlayerDirection() {
            return AnimProgress > 0.4f && AnimProgress < 0.6f;
        }

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
            handPosition = Main.GetPlayerArmPosition(Projectile);
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
                    scale * Projectile.scale, 
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

        public override void SetDefaults() {
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = 500;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.aiStyle = ProjAIStyleID.Spear;
            amountAllowedToHit = 2;
            swordHeight = 100;
            swordWidth = 30;
        }

        public override bool? CanDamage() {
            return AnimProgress > 0.4f && AnimProgress < 0.6f && freezeFrame <= 0 ? null : false;
        }

        public override void AI() {
            var player = Main.player[Projectile.owner];
            Projectile.aiStyle = -1;

            if (Projectile.numUpdates == -1 && freezeFrame > 0) {
                freezeFrame--;
                if (freezeFrame != 1) {
                    player.itemAnimation++;
                    player.itemTime++;
                }
            }

            var aequus = player.GetModPlayer<AequusPlayer>();

            player.heldProj = Projectile.whoAmI;
            if (!_init) {
                Projectile.scale = 1f;
                DoInitialize(player, player.Aequus());
                baseSwordScale = Projectile.scale;
                Projectile.netUpdate = true;
                _init = true;
            }
            if (player.ownedProjectileCounts[Type] > 1 || swingTime < 2) {
                Projectile.Kill();
                player.ownedProjectileCounts[Type]--;
                aequus.itemCombo = (ushort)(combo == 0 ? swingTimeMax : 0);
                TimesSwinged++;
            }

            if (ShouldUpdatePlayerDirection()) {
                UpdatePlayerDirection(player);
            }

            if (!player.frozen && !player.stoned) {
                var arm = Main.GetPlayerArmPosition(Projectile);
                float progress = AnimProgress;
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
                if (Main.netMode != NetmodeID.Server) {
                    SetArmRotation(player, progress, swingProgress);
                }
                Projectile.scale = scale;
                animationGFXOutOffset = (int)outer;

                if (freezeFrame == 0) {
                    swingTime--;
                }
            }
        }

        public virtual void UpdateSwing(float progress, float interpolatedSwingProgress) {
        }

        public virtual float SwingProgress(float progress) {
            return progress;
        }
        public static float GenericSwing3(float progress) {
            return progress >= 0.5f ? 0.5f + (0.5f - MathF.Pow(2f, 20f * (0.5f - (progress - 0.5f)) - 10f) / 2f) : MathF.Pow(2f, 20f * progress - 10f) / 2f;
        }
        public static float GenericSwing2(float progress, float pow = 2f) {
            if (progress > 0.5f) {
                return 0.5f - GenericSwing2(0.5f - (progress - 0.5f), pow) + 0.5f;
            }
            return ((float)Math.Sin(Math.Pow(progress, pow) * MathHelper.TwoPi - MathHelper.PiOver2) + 1f) / 2f;
        }
        public static float GenericSwing1(float progress, float pow = 2f, float startSwishing = 0.15f) {
            float oldProg = progress;
            float max = 1f - startSwishing;
            if (progress < startSwishing) {
                progress *= (float)Math.Pow(progress / startSwishing, pow);
            }
            else if (progress > max) {
                progress -= max;
                progress = startSwishing - progress;
                progress *= (float)Math.Pow(progress / startSwishing, pow);
                progress = startSwishing - progress;
                progress += max;
            }
            return MathHelper.Clamp((float)Math.Sin(progress * MathHelper.Pi - MathHelper.PiOver2) / 2f + 0.5f, 0f, oldProg);
        }
        public virtual Vector2 GetOffsetVector(float progress) {
            return BaseAngleVector.RotatedBy((progress * MathHelper.Pi - MathHelper.PiOver2) * -swingDirection);
        }
        public virtual float GetScale(float progress) {
            return baseSwordScale;
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

        private void DoInitialize(Player player, AequusPlayer aequus) {
            AngleVector = Projectile.velocity;
            combo = aequus.itemCombo;
            if (player.whoAmI == Projectile.owner) {
                Helper.CappedMeleeScale(Projectile);
            }

            swingDirection = 1;
            UpdatePlayerDirection(player);
            swingDirection *= Projectile.direction;
            swingTimeMax = Main.player[Projectile.owner].itemAnimationMax;

            // Flip direction
            if (aequus.itemCombo > 0) {
                swingDirection *= -1;
            }

            Initialize(player, aequus);

            baseSwordScale = Projectile.scale;
            Main.player[Projectile.owner].itemTime = swingTimeMax + 1;
            Main.player[Projectile.owner].itemTimeMax = swingTimeMax + 1;
            Main.player[Projectile.owner].itemAnimation = swingTimeMax + 1;
            Main.player[Projectile.owner].itemAnimationMax = swingTimeMax + 1;

            swingTimeMax *= Projectile.extraUpdates + 1;
            swingTime = swingTimeMax;
        }
        protected virtual void Initialize(Player player, AequusPlayer aequus) {
        }

        protected virtual void SetArmRotation(Player player, float progress, float swingProgress) {
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

            if (armRotation > 1.1f) {
                player.bodyFrame.Y = 56;
            }
            else if (armRotation > 0.5f) {
                player.bodyFrame.Y = 56 * 2;
            }
            else if (armRotation < -0.5f) {
                player.bodyFrame.Y = 56 * 4;
            }
            else {
                player.bodyFrame.Y = 56 * 3;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            amountAllowedToHit--;
        }

        public override bool? CanHitNPC(NPC target) {
            return amountAllowedToHit > 0 ? null : false;
        }

        public override void SendExtraAI(BinaryWriter writer) {
            writer.Write((byte)freezeFrame);
            writer.Write(swingDirection == -1);
            writer.Write(combo);
        }

        public override void ReceiveExtraAI(BinaryReader reader) {
            freezeFrame = reader.ReadByte();
            swingDirection = reader.ReadBoolean() ? -1 : 1;
            combo = reader.ReadInt32();
        }

        public override void PostDraw(Color lightColor) {
            //DrawDebug();
        }
    }
}