using Aequus.Common.Players.Backpacks;
using System.Collections.Generic;
using Terraria.GameContent.Golf;
using Terraria.Physics;

namespace Aequus.Common.Golfing;

public class GolfingSystem : ModSystem {
    public static List<int> FakeGolfBalls { get; private set; } = new();

    public override void Load() {
        On_Player.GetPreferredGolfBallToUse += On_Player_GetPreferredGolfBallToUse;
        On_GolfHelper.StepGolfBall += On_GolfHelper_StepGolfBall;
    }

    public override void PostSetupContent() {
        foreach (var modProjectile in Mod.GetContent<ModProjectile>()) {
            if (modProjectile is IGolfBallProjectile && !ProjectileID.Sets.IsAGolfBall[modProjectile.Type]) {
                FakeGolfBalls.Add(modProjectile.Type);
            }
        }
    }

    private static BallStepResult On_GolfHelper_StepGolfBall(On_GolfHelper.orig_StepGolfBall orig, Entity entity, ref float angularVelocity) {
        if (entity is Projectile projectile && projectile.ModProjectile is IGolfBallProjectile golfBallProjectile && golfBallProjectile.StepGolfBall(ref angularVelocity, out var result)) {
            return result;
        }
        return orig(entity, ref angularVelocity);
    }

    private static void On_Player_GetPreferredGolfBallToUse(On_Player.orig_GetPreferredGolfBallToUse orig, Player player, out int projType) {
        orig(player, out projType);
        var heldItem = player.HeldItem;
        if (!heldItem.IsAir && heldItem.shoot == projType) {
            return;
        }

        if (!heldItem.IsAir && (ProjectileLoader.GetProjectile(heldItem.shoot) as IGolfBallProjectile)?.OverrideGolfBallId(player, heldItem, projType) == true) {
            projType = heldItem.shoot;
            return;
        }

        for (int i = 0; i < Main.InventorySlotsTotal; i++) {
            var item = player.inventory[i];
            if (!item.IsAir && (ProjectileLoader.GetProjectile(item.shoot) as IGolfBallProjectile)?.OverrideGolfBallId(player, item, projType) == true) {
                projType = item.shoot;
            }
        }

        BackpackLoader.GetPreferredGolfBallToUse(player, player.GetModPlayer<AequusPlayer>().backpacks, ref projType);
    }

    /// <summary>
    /// Sets all fake golf balls to be counted or not counted as golf balls.
    /// </summary>
    /// <param name="value"></param>
    public static void SetGolfBallStatus(bool value) {
        foreach (var i in FakeGolfBalls) {
            ProjectileID.Sets.IsAGolfBall[i] = value;
        }
    }
}