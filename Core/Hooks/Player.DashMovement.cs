using AequusRemake.Core.Entities.Players;
using System;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows for custom dash accessories added by AequusRemake to update dash movement.</summary>
    private static void On_Player_DashMovement(On_Player.orig_DashMovement orig, Player player) {
        orig(player);

        if (player.mount.Active || player.dash != -1 || !player.TryGetModPlayer<AequusPlayer>(out var AequusRemakePlayer) || AequusRemakePlayer.DashData == null) {
            return;
        }

        CustomDashData dashData = AequusRemakePlayer.DashData;

        if (player.dashDelay > 0) {
            dashData.OnUpdateDashDelay(player, AequusRemakePlayer);
        }
        else if (player.dashDelay < 0) {
            float dashSpeed = dashData.DashHaltSpeed;
            float dashSpeedPenalty = dashData.DashHaltSpeedMultiplier;
            float movementSpeed = Math.Max(player.accRunSpeed, player.maxRunSpeed);
            float movementSpeedPenalty = 0.96f;

            int dashDelay = dashData.DashDelay;
            player.doorHelper.AllowOpeningDoorsByVelocityAloneForATime(dashDelay * 3);
            player.vortexStealthActive = false;
            dashData.OnUpdateRampDown(player, AequusRemakePlayer);
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
            dashData.OnApplyDash(player, AequusRemakePlayer);
        }
        else {
            AequusRemakePlayer.DoCommonDashHandle(player, out int direction, out bool dashing, dashData);
            if (dashing) {
                player.velocity.X = dashData.DashSpeed * direction;

                Point frontPoint = (player.Center + new Vector2(direction * player.width / 2 + 2, player.gravDir * -player.height / 2f + player.gravDir * 2f)).ToTileCoordinates();
                Point otherFrontPoint = (player.Center + new Vector2(direction * player.width / 2 + 2, 0f)).ToTileCoordinates();
                if (WorldGen.SolidOrSlopedTile(frontPoint.X, frontPoint.Y) || WorldGen.SolidOrSlopedTile(otherFrontPoint.X, otherFrontPoint.Y)) {
                    player.velocity.X /= 2f;
                }
                player.dashDelay = -1;

                dashData.OnDashVelocityApplied(player, AequusRemakePlayer, direction);
            }
        }
    }
}
