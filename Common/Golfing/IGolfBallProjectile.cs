using Terraria.GameContent.Golf;
using Terraria.Physics;

namespace Aequus.Common.Golfing;

public interface IGolfBallProjectile {
    /// <summary>
    /// Return true to override <paramref name="projType"/> with this projectile Id.
    /// By default, returns true when <paramref name="projType"/> equals <see cref="ProjectileID.DirtGolfBall"/> (Default golf ball).
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="golfBall">The golf ball item.</param>
    /// <param name="projType">The projectile Id of the previously chosen golf ball.</param>
    /// <returns></returns>
    System.Boolean OverrideGolfBallId(Player player, Item golfBall, System.Int32 projType) {
        return projType == ProjectileID.DirtGolfBall;
    }

    System.Int32 GolfBallHitDustId => 31;

    System.Boolean PreHit(Vector2 shotVector) {
        return true;
    }
    void OnHit(Vector2 velocity, GolfHelper.ShotStrength shotStrength);

    System.Boolean StepGolfBall(ref System.Single angularVelocity, out BallStepResult result) {
        result = default;
        return false;
    }
}