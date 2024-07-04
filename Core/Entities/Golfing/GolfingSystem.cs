using System.Collections.Generic;

namespace AequusRemake.Core.Entities.Golfing;

public class GolfingSystem : ModSystem {
    public static List<int> FakeGolfBalls { get; private set; } = new();

    public override void PostSetupContent() {
        foreach (var modProjectile in Mod.GetContent<ModProjectile>()) {
            if (modProjectile is IGolfBallProjectile && !ProjectileID.Sets.IsAGolfBall[modProjectile.Type]) {
                FakeGolfBalls.Add(modProjectile.Type);
            }
        }
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