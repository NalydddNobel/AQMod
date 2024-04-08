using Aequus.Common.Golfing;
using Terraria.GameContent.Golf;
using Terraria.Physics;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static BallStepResult On_GolfHelper_StepGolfBall(On_GolfHelper.orig_StepGolfBall orig, Entity entity, ref float angularVelocity) {
        if (entity is Projectile projectile && projectile.ModProjectile is IGolfBallProjectile golfBallProjectile && golfBallProjectile.StepGolfBall(ref angularVelocity, out var result)) {
            return result;
        }
        return orig(entity, ref angularVelocity);
    }
}
