using System;

namespace Aequu2.Core.Entities.Projectiles;

[Obsolete]
public abstract class HeldProjBase : ModProjectile {
    public float armRotation;
    protected virtual void SetArmRotation(Player player) {
        if (armRotation > 1.1f) {
            player.bodyFrame.Y = 56;
        }
        else if (armRotation > 0.5f) {
            player.bodyFrame.Y = 56 * 2;
        }
        else if (armRotation < -0.5f) {
            player.bodyFrame.Y = 56 * 4;
        }
        else {
            player.bodyFrame.Y = 56 * 3;
        }
    }
}