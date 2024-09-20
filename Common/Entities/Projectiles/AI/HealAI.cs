using System;

namespace Aequus.Common.Entities.Projectiles.AI;

public static class HealAI {
    public static void AI(Projectile projectile, Player target, Action<Player> OnHealEvent) {
        Vector2 center = projectile.Center;
        Vector2 targetDifference = target.Center - center;
        float targetDistance = targetDifference.Length();

        if (targetDistance < 50f && projectile.Colliding(projectile.getRect(), target.getRect())) {
            if (projectile.owner == Main.myPlayer && !Main.player[Main.myPlayer].moonLeech) {
                OnHealEvent(target);
            }
            projectile.Kill();
        }

        float movementMultiplier = 4f / targetDistance;
        projectile.velocity = Vector2.Lerp(projectile.velocity, targetDifference * movementMultiplier, 1f / 16f);
    }

}
