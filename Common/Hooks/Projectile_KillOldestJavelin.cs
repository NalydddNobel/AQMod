using Aequus.Core.Entities.Projectiles;

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void On_Projectile_KillOldestJavelin(On_Projectile.orig_KillOldestJavelin orig, int protectedProjectileIndex, int projectileType, int targetNPCIndex, Point[] bufferForScan) {
        if (protectedProjectileIndex >= 0 && protectedProjectileIndex < Main.maxProjectiles) {
            Main.projectile[protectedProjectileIndex].netUpdate = true;
            Main.projectile[protectedProjectileIndex].GetGlobalProjectile<ProjectileItemData>().NoSpecialEffects = true;
        }
        orig(protectedProjectileIndex, projectileType, targetNPCIndex, bufferForScan);
    }
}
