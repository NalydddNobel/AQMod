using Aequu2.Core.Entities.Golfing;
using Terraria.GameContent.Golf;
using Terraria.Physics;

namespace Aequu2.Core.Hooks;

public partial class TerrariaHooks {
    /// <summary>Allows custom golf ball prediction lines for projectiles which implement <see cref="IGolfBallProjectile"/>.</summary>
    private static BallStepResult On_GolfHelper_StepGolfBall(On_GolfHelper.orig_StepGolfBall orig, Entity entity, ref float angularVelocity) {
        if (entity is Projectile projectile && projectile.ModProjectile is IGolfBallProjectile golfBallProjectile && golfBallProjectile.StepGolfBall(ref angularVelocity, out var result)) {
            return result;
        }
        return orig(entity, ref angularVelocity);
    }
}
