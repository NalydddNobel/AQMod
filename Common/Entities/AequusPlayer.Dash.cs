using System;

namespace Aequus.Common.Entities;

public class DashPlayer : ModPlayer {
    public static readonly float DashHaltSpeed = 12f;
    /// <summary>Multiplier used to reduce velocity when above the dash's halting speed.</summary>
    public static readonly float DashHaltSpeedMultiplier = 0.992f;

    public static readonly int DashDelay = 20;

    public ICustomDashProvider? Dash { get; set; }

    public bool HasCustomDash => Dash != null;

    public override void FrameEffects() {
        if (Player.dashDelay != 0 && Player.dash == -1 && Dash != null) {
            Dash.OnPlayerFrame(Player);
        }
    }

    public override void Load() {
        On_Player.DashMovement += On_Player_DashMovement;
        On_Player.UpdateVisibleAccessories += On_Player_UpdateVisibleAccessories;
    }

    static void On_Player_DashMovement(On_Player.orig_DashMovement orig, Player player) {
        orig(player);

        if (player.mount.Active || player.dash != -1 || !player.TryGetModPlayer<DashPlayer>(out var dashPlayer) || dashPlayer.Dash == null) {
            return;
        }

        ICustomDashProvider dash = dashPlayer.Dash;

        if (player.dashDelay > 0) {
            dash.OnUpdateDashDelay(player);
        }
        else if (player.dashDelay < 0) {
            TryStartDash(player, dash);
        }
        else {
            DoCommonDashHandle(player, out int direction, out bool dashing, dash);
            if (dashing) {
                ApplyDashVelocity(player, dash, direction);
            }
        }

        static void TryStartDash(Player player, ICustomDashProvider dash) {
            float dashSpeed = DashHaltSpeed;
            float dashSpeedPenalty = DashHaltSpeedMultiplier;
            float movementSpeed = Math.Max(player.accRunSpeed, player.maxRunSpeed);
            float movementSpeedPenalty = 0.96f;

            int dashDelay = DashDelay;
            player.doorHelper.AllowOpeningDoorsByVelocityAloneForATime(dashDelay * 3);
            player.vortexStealthActive = false;

            dash.OnUpdateRampDown(player);

            if (player.velocity.X > dashSpeed || player.velocity.X < -dashSpeed) {
                player.velocity.X *= dashSpeedPenalty;
                return;
            }
            if (player.velocity.X > movementSpeed || player.velocity.X < -movementSpeed) {
                player.velocity.X *= movementSpeedPenalty;
                return;
            }
            player.dashDelay = dashDelay;
            if (player.velocity.X < 0f) {
                player.velocity.X = -movementSpeed;
            }
            else if (player.velocity.X > 0f) {
                player.velocity.X = movementSpeed;
            }

            dash.OnApplyDash(player);
        }

        static void DoCommonDashHandle(Player player, out int dir, out bool dashing, ICustomDashProvider dashData) {
            dir = 0;
            dashing = false;
            if (player.dashTime > 0) {
                player.dashTime--;
            }
            if (player.dashTime < 0) {
                player.dashTime++;
            }
            if (player.controlRight && player.releaseRight) {
                if (player.dashTime > 0) {
                    dir = 1;
                    dashing = true;
                    player.dashTime = 0;
                    player.timeSinceLastDashStarted = 0;
                    dashData.OnHandledStart(player, dir);
                }
                else {
                    player.dashTime = 15;
                }
            }
            else if (player.controlLeft && player.releaseLeft) {
                if (player.dashTime < 0) {
                    dir = -1;
                    dashing = true;
                    player.dashTime = 0;
                    player.timeSinceLastDashStarted = 0;
                    dashData.OnHandledStart(player, dir);
                }
                else {
                    player.dashTime = -15;
                }
            }
        }

        static void ApplyDashVelocity(Player player, ICustomDashProvider dash, int dir) {
            player.velocity.X = dash.DashSpeed * dir;

            Point frontPoint = (player.Center + new Vector2(dir * player.width / 2 + 2, player.gravDir * -player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
            Point otherFrontPoint = (player.Center + new Vector2(dir * player.width / 2 + 2, 0f)).ToTileCoordinates();
            if (WorldGen.SolidOrSlopedTile(frontPoint.X, frontPoint.Y) || WorldGen.SolidOrSlopedTile(otherFrontPoint.X, otherFrontPoint.Y)) {
                player.velocity.X /= 2f;
            }
            player.dashDelay = -1;

            dash.OnDashVelocityApplied(player, dir);
        }
    }

    private static void On_Player_UpdateVisibleAccessories(On_Player.orig_UpdateVisibleAccessories orig, Player player) {
        if (!player.TryGetModPlayer<DashPlayer>(out var dashPlayer)) {
            orig(player);
            return;
        }

        ICustomDashProvider? dashData = player.dashDelay < 0 && player.dash == -1 && dashPlayer.Dash != null ? dashPlayer.Dash : null;
        dashData?.PreUpdateVisibleAccessories(player);
        orig(player);
        dashData?.PostUpdateVisibleAccessories(player);
    }
}
