using System;
using System.IO;

namespace Aequus.Common.Projectiles;

public abstract class LegacyHeldSlashingSwordProjectile : LegacyHeldSwordProjectile {
    protected Boolean _halfWayMark;

    public Int32 swingDirection;

    /// <summary>
    /// Stat which determines how far the sword's laser hitbox will reach, in pixels.
    /// </summary>
    public Int32 swordHeight;
    /// <summary>
    /// Stat which determines how wide the sword's laser hitbox will be, in pixels.
    /// </summary>
    public Int32 swordWidth;
    /// <summary>
    /// Outward offset, only effects rendering.
    /// </summary>
    public Int32 gfxOutOffset;
    /// <summary>
    /// Outward offset, only effects rendering. Updated every game update.
    /// </summary>
    public Int32 animationGFXOutOffset;

    public Boolean playedSound;

    public Int32 freezeFrame;

    protected Single rotationOffset;
    protected Single lastAnimProgress;

    private Vector2 angleVector;
    public Vector2 AngleVector { get => angleVector; set => angleVector = Vector2.Normalize(value); }
    public Int32 GFXOutOffset => gfxOutOffset + animationGFXOutOffset;


    public virtual Boolean ShouldUpdatePlayerDirection() {
        return AnimProgress > 0.4f && AnimProgress < 0.6f;
    }

    public override void SetDefaults() {
        base.SetDefaults();
        swordHeight = 100;
        swordWidth = 30;
    }

    public override Boolean? CanDamage() {
        return AnimProgress > 0.4f && AnimProgress < 0.6f && freezeFrame <= 0 ? null : false;
    }

    protected override void UpdateSword(Player player, AequusPlayer aequus, Single progress) {
        var arm = Main.GetPlayerArmPosition(Projectile);
        if (!_halfWayMark && progress >= 0.5f) {
            if (Projectile.numUpdates != -1 || freezeFrame > 0) {
                return;
            }
            progress = 0.5f;
            _halfWayMark = true;
        }
        lastAnimProgress = progress;
        InterpolateSword(progress, out var angleVector, out Single swingProgress, out Single scale, out Single outer);
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
        animationGFXOutOffset = (Int32)outer;

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
        if (aequus.TryGetTimer(SwordSwingFlipTimer, out var timer) && timer.Active) {
            swingDirection *= -1;
        }

        InitializeSword(player, aequus);
    }

    protected virtual void InitializeSword(Player player, AequusPlayer aequus) {

    }

    public virtual void UpdateSwing(Single progress, Single interpolatedSwingProgress) {
    }

    public virtual Vector2 GetOffsetVector(Single progress) {
        return BaseAngleVector.RotatedBy((progress * MathHelper.Pi - MathHelper.PiOver2) * -swingDirection);
    }

    public virtual Single GetVisualOuter(Single progress, Single swingProgress) {
        return animationGFXOutOffset;
    }

    public void InterpolateSword(Single progress, out Vector2 offsetVector, out Single swingProgress, out Single scale, out Single outer) {
        swingProgress = SwingProgress(progress);
        offsetVector = GetOffsetVector(swingProgress);
        scale = GetScale(swingProgress);
        outer = (Int32)GetVisualOuter(progress, swingProgress);
    }
    public override Boolean? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
        var center = Main.player[Projectile.owner].Center;
        //Helper.DebugDust(center - AngleVector * 20f);
        //Helper.DebugDust(center + AngleVector * (swordHeight * Projectile.scale * baseSwordScale), DustID.CursedTorch);
        return ExtendCollision.LineHitbox(center - AngleVector * 20f, center + AngleVector * (swordHeight * Projectile.scale * baseSwordScale), targetHitbox, swordWidth * Projectile.scale * baseSwordScale);
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

    protected virtual void UpdateArmRotation(Player player, Single progress, Single swingProgress) {
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
        writer.Write((Byte)freezeFrame);
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
        DrawHelper.DrawLine(center - Main.screenPosition + startOffset, center + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, 4f, Color.Green);
        DrawHelper.DrawLine(center + AngleVector.RotatedBy(-MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f - Main.screenPosition, center + AngleVector.RotatedBy(MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f - Main.screenPosition, 4f, Color.Red);
        DrawHelper.DrawLine(center + AngleVector.RotatedBy(-MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, center + AngleVector.RotatedBy(MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, 4f, Color.Red);
        DrawHelper.DrawLine(center + AngleVector.RotatedBy(-MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, center + AngleVector.RotatedBy(-MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f - Main.screenPosition, 4f, Color.Orange);
        DrawHelper.DrawLine(center + AngleVector.RotatedBy(MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f + AngleVector * swordHeight * Projectile.scale * baseSwordScale - Main.screenPosition, center + AngleVector.RotatedBy(MathHelper.PiOver2) * swordWidth * Projectile.scale * baseSwordScale / 2f - Main.screenPosition, 4f, Color.Orange);
    }

    public void GetSwordDrawInfo(out Texture2D texture, out Vector2 handPosition, out Rectangle frame, out Single rotationOffset, out Vector2 origin, out SpriteEffects effects) {
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

    protected void DrawSword(Texture2D texture, Vector2 handPosition, Rectangle frame, Color color, Single rotationOffset, Vector2 origin, SpriteEffects effects) {
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

    protected void DrawSwordAfterImages(Texture2D texture, Vector2 handPosition, Rectangle frame, Color color, Single rotationOffset, Vector2 origin, SpriteEffects effects, Single loopProgress = 0.07f, Single interpolationValue = -0.01f) {
        Single trailAlpha = 1f;
        for (Single f = lastAnimProgress; f > 0.05f && f < 0.95f && trailAlpha > 0f; f += interpolationValue) {
            InterpolateSword(f, out var offsetVector, out Single _, out Single scale, out Single outer);
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

    protected void DrawSwordTipFlare(Vector2 handPosition, Single swordTipLength, Vector2 flareSize, Color flareColor, Single bloomScale, Color bloomColor) {
        var flare = AequusTextures.Flare.Value;
        var flareOrigin = flare.Size() / 2f;
        var flarePosition = handPosition - Main.screenPosition + AngleVector * swordTipLength * baseSwordScale;
        Main.EntitySpriteDraw(
            AequusTextures.BloomStrong,
            flarePosition,
            null,
            bloomColor,
            0f,
            AequusTextures.BloomStrong.Size() / 2f,
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