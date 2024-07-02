using System;

namespace Aequus.Core.Entities.NPCs;

public abstract class AIJellyfish : ModNPC {
    public virtual bool CanShock() {
        return Main.expertMode;
    }

    /// <summary>
    /// When the target is within this range, the delay for activating the electric state is dramatically decreased.
    /// <para><see cref="shockStartRampUp"/> is used to decrease the cooldown of the attack.</para>
    /// <para><see cref="shockEndRampUp"/> is used to increase the duration of the attack.</para>
    /// <para>Defaults to 150.</para>
    /// </summary>
    public int shockRampUpDistance = 150;
    /// <summary>
    /// Decreases the cooldown of the shock attack when within <see cref="shockRampUpDistance"/>.
    /// <para>Defaults to 2f.</para>
    /// </summary>
    public float shockStartRampUp = 2f;
    /// <summary>
    /// Increases the duration of the shock attack when within <see cref="shockRampUpDistance"/>.
    /// <para>Defaults to 0.25f.</para>
    /// </summary>
    public float shockEndRampUp = 0.25f;

    /// <summary>
    /// The duration of the shock attack's cooldown.
    /// <para>Defaults to 420. (7 seconds)</para>
    /// </summary>
    public int shockAttackCooldown = 420;
    /// <summary>
    /// The duration of the shock attack.
    /// <para>Defaults to 120. (2 seconds)</para>
    /// </summary>
    public int shockAttackLength = 120;

    /// <summary>
    /// Defaults to 7.
    /// </summary>
    public float dashSpeed = 7f;
    /// <summary>
    /// Multiplier on velocity when it wants to slow down for a dash attack.
    /// <para>Defaults to 0.98f.</para>
    /// </summary>
    public float dashAttackSlowdown = 0.98f;
    /// <summary>
    /// How slow the Jellyfish must be moving in order for it to be allowed to dash.
    /// <para>Defaults to 0.2f.</para>
    /// </summary>
    public float dashAttackSpeedThreshold = 0.2f;

    public bool InShockState => NPC.wet && NPC.ai[1] == 1f;

    public virtual void CastLights(float lightIntensity) {
    }

    public virtual void UpdateShockState() {
        if (NPC.target >= 0 && Main.player[NPC.target].wet && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height)) {
            if (NPC.Distance(Main.player[NPC.target].Center) < shockRampUpDistance) {
                if (NPC.ai[1] == 0f) {
                    NPC.ai[2] += shockStartRampUp;
                }
                else {
                    NPC.ai[2] -= shockEndRampUp;
                }
            }
        }
        if (InShockState) {
            NPC.dontTakeDamage = true;
            NPC.ai[2] += 1f;
            if (NPC.ai[2] >= shockAttackLength) {
                NPC.ai[1] = 0f;
            }
        }
        else {
            NPC.ai[2] += 1f;
            if (NPC.ai[2] >= shockAttackCooldown) {
                NPC.ai[1] = 1f;
                NPC.ai[2] = 0f;
            }
        }
    }

    public virtual void HandleWetCollisions() {
        int i = (int)NPC.Center.X / 16;
        int j = (int)(NPC.position.Y + NPC.height) / 16;
        var centerTile = Framing.GetTileSafely(i, j);
        var bottomTile = Framing.GetTileSafely(i, j + 1);
        if (centerTile.TopSlope) {
            if (centerTile.LeftSlope) {
                NPC.direction = -1;
                NPC.velocity.X = Math.Abs(NPC.velocity.X) * -1f;
            }
            else {
                NPC.direction = 1;
                NPC.velocity.X = Math.Abs(NPC.velocity.X);
            }
        }
        else if (bottomTile.TopSlope) {
            if (bottomTile.LeftSlope) {
                NPC.direction = -1;
                NPC.velocity.X = Math.Abs(NPC.velocity.X) * -1f;
            }
            else {
                NPC.direction = 1;
                NPC.velocity.X = Math.Abs(NPC.velocity.X);
            }
        }
        if (NPC.collideX) {
            NPC.velocity.X *= -1f;
            NPC.direction *= -1;
        }
        if (NPC.collideY) {
            if (NPC.velocity.Y > 0f) {
                NPC.velocity.Y = Math.Abs(NPC.velocity.Y) * -1f;
                NPC.directionY = -1;
                NPC.ai[0] = -1f;
            }
            else if (NPC.velocity.Y < 0f) {
                NPC.velocity.Y = Math.Abs(NPC.velocity.Y);
                NPC.directionY = 1;
                NPC.ai[0] = 1f;
            }
        }
    }

    public virtual void WetMovement() {
        NPC.velocity.X += NPC.direction * 0.02f;
        NPC.rotation = NPC.velocity.X * 0.4f;
        if (NPC.velocity.X < -1f || NPC.velocity.X > 1f) {
            NPC.velocity.X *= 0.95f;
        }
        if (NPC.ai[0] == -1f) {
            NPC.velocity.Y -= 0.01f;
            if (NPC.velocity.Y < -1f) {
                NPC.ai[0] = 1f;
            }
        }
        else {
            NPC.velocity.Y += 0.01f;
            if (NPC.velocity.Y > 1f) {
                NPC.ai[0] = -1f;
            }
        }
        int i = (int)(NPC.position.X + NPC.width / 2) / 16;
        int j = (int)(NPC.position.Y + NPC.height / 2) / 16;
        if (Framing.GetTileSafely(i, j - 1).LiquidAmount > 128) {
            if (Framing.GetTileSafely(i, j + 1).HasTile) {
                NPC.ai[0] = -1f;
            }
            else if (Framing.GetTileSafely(i, j + 2).HasTile) {
                NPC.ai[0] = -1f;
            }
        }
        else {
            NPC.ai[0] = 1f;
        }
        if (NPC.velocity.Y > 1.2f || NPC.velocity.Y < -1.2f) {
            NPC.velocity.Y *= 0.99f;
        }
    }

    public virtual bool HandleDashAttack() {
        NPC.TargetClosest(faceTarget: false);
        if (Main.player[NPC.target].wet && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height)) {
            NPC.localAI[2] = 1f;
            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
            NPC.velocity *= dashAttackSlowdown;
            if (NPC.velocity.X > -dashAttackSpeedThreshold && NPC.velocity.X < dashAttackSpeedThreshold && NPC.velocity.Y > -dashAttackSpeedThreshold && NPC.velocity.Y < dashAttackSpeedThreshold) {
                NPC.TargetClosest();
                PerformDashAttack();
            }
            return true;
        }
        return false;
    }

    public virtual void PerformDashAttack() {
        var difference = Main.player[NPC.target].Center - NPC.Center;
        NPC.velocity = Vector2.Normalize(difference) * dashSpeed;
    }

    public virtual void DryMovement() {
        NPC.rotation += NPC.velocity.X * 0.1f;
        if (NPC.velocity.Y == 0f) {
            NPC.velocity.X *= 0.98f;
            if (NPC.velocity.X > -0.01f && NPC.velocity.X < 0.01f) {
                NPC.velocity.X = 0f;
            }
        }
        NPC.velocity.Y += 0.2f;
        if (NPC.velocity.Y > 10f) {
            NPC.velocity.Y = 10f;
        }
        NPC.ai[0] = 1f;
    }

    public override void AI() {
        if (!InShockState) {
            NPC.dontTakeDamage = false;
        }

        if (CanShock()) {
            if (NPC.wet) {
                UpdateShockState();
            }
            else {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
            }
        }

        CastLights(InShockState ? 1.5f : 1f);

        if (NPC.direction == 0) {
            NPC.TargetClosest();
        }

        // Ignore movement operations when in the shock state
        if (InShockState) {
            return;
        }

        if (NPC.wet) {
            HandleWetCollisions();

            if (!NPC.friendly && HandleDashAttack()) {
                return;
            }

            NPC.localAI[2] = 0f;
            WetMovement();
        }
        else {
            DryMovement();
        }
    }
}