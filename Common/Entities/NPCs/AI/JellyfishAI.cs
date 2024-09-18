using System;

namespace Aequus.Common.Entities.NPCs.AI;

public abstract class JellyfishAI : ModNPC {
    public const int DefaultShockRampUpDistance = 150;
    public const float DefaultShockStartRampUp = 2f;
    public const float DefaultShockEndRampUp = 0.25f;
    public const int DefaultShockAttackCooldown = 420;
    public const int DefaultShockAttackLength = 120;
    public const float DefaultDashSpeed = 8f;
    public const float DefaultDashAttackSlowdown = 0.98f;
    public const float DefaultDashAttackSpeedThreshold = 0.2f;

    public static void AI(IJellyfishAIProvider jellyfish) {
        NPC NPC = jellyfish.NPC;

        if (!jellyfish.InShockState) {
            NPC.dontTakeDamage = false;
        }

        if (jellyfish.CanShock()) {
            if (NPC.wet) {
                jellyfish.UpdateShockState();
            }
            else {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
            }
        }

        jellyfish.CastLights(jellyfish.InShockState ? 1.5f : 1f);

        if (NPC.direction == 0) {
            NPC.TargetClosest();
        }

        // Ignore movement operations when in the shock state
        if (jellyfish.InShockState) {
            return;
        }

        if (NPC.wet) {
            jellyfish.HandleWetCollisions();

            if (!NPC.friendly && jellyfish.HandleDashAttack()) {
                return;
            }

            NPC.localAI[2] = 0f;
            jellyfish.WetMovement();
        }
        else {
            jellyfish.DryMovement();
        }
    }
}

public interface IJellyfishAIProvider {
    /// <summary>
    /// When the target is within this range, the delay for activating the electric state is dramatically decreased.
    /// <para><see cref="ShockStartRampUp"/> is used to decrease the cooldown of the attack.</para>
    /// <para><see cref="ShockEndRampUp"/> is used to increase the duration of the attack.</para>
    /// </summary>
    int ShockRampUpDistance => JellyfishAI.DefaultShockRampUpDistance;
    /// <summary>Decreases the cooldown of the shock attack when within <see cref="ShockRampUpDistance"/>.</summary>
    float ShockStartRampUp => JellyfishAI.DefaultShockStartRampUp;
    /// <summary>Increases the duration of the shock attack when within <see cref="ShockRampUpDistance"/>.</summary>
    float ShockEndRampUp => JellyfishAI.DefaultShockEndRampUp;

    /// <summary>The duration of the shock attack's cooldown.</summary>
    int ShockAttackCooldown => JellyfishAI.DefaultShockAttackCooldown;
    /// <summary>The duration of the shock attack.</summary>
    int ShockAttackLength => JellyfishAI.DefaultShockAttackLength;

    float DashSpeed => JellyfishAI.DefaultDashSpeed;
    /// <summary>Multiplier on velocity when it wants to slow down for a dash attack.</summary>
    float DashAttackSlowdown => JellyfishAI.DefaultDashAttackSlowdown;
    /// <summary>How slow the Jellyfish must be moving in order for it to be allowed to dash.</summary>
    float DashAttackSpeedThreshold => JellyfishAI.DefaultDashAttackSpeedThreshold;

    bool InShockState => NPC.wet && NPC.ai[1] == 1f;

    abstract NPC NPC { get; }

    virtual bool CanShock() {
        return Main.expertMode;
    }

    virtual void CastLights(float lightIntensity) {
    }

    virtual void UpdateShockState() {
        if (NPC.target >= 0 && Main.player[NPC.target].wet && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height)) {
            if (NPC.Distance(Main.player[NPC.target].Center) < ShockRampUpDistance) {
                if (NPC.ai[1] == 0f) {
                    NPC.ai[2] += ShockStartRampUp;
                }
                else {
                    NPC.ai[2] -= ShockEndRampUp;
                }
            }
        }
        if (InShockState) {
            NPC.dontTakeDamage = true;
            NPC.ai[2] += 1f;
            if (NPC.ai[2] >= ShockAttackLength) {
                NPC.ai[1] = 0f;
            }
        }
        else {
            NPC.ai[2] += 1f;
            if (NPC.ai[2] >= ShockAttackCooldown) {
                NPC.ai[1] = 1f;
                NPC.ai[2] = 0f;
            }
        }
    }

    virtual void HandleWetCollisions() {
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

    virtual void WetMovement() {
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

    virtual bool HandleDashAttack() {
        NPC.TargetClosest(faceTarget: false);
        if (Main.player[NPC.target].wet && !Main.player[NPC.target].dead && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height)) {
            NPC.localAI[2] = 1f;
            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
            NPC.velocity *= DashAttackSlowdown;
            if (NPC.velocity.X > -DashAttackSpeedThreshold && NPC.velocity.X < DashAttackSpeedThreshold && NPC.velocity.Y > -DashAttackSpeedThreshold && NPC.velocity.Y < DashAttackSpeedThreshold) {
                NPC.TargetClosest();
                PerformDashAttack();
            }
            return true;
        }
        return false;
    }

    virtual void PerformDashAttack() {
        var difference = Main.player[NPC.target].Center - NPC.Center;
        NPC.velocity = Vector2.Normalize(difference) * DashSpeed;
    }

    virtual void DryMovement() {
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
}