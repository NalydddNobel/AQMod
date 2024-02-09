namespace Aequus.Common.Projectiles;

public class Javelins : ILoad {
    private static void On_Projectile_KillOldestJavelin(On_Projectile.orig_KillOldestJavelin orig, int protectedProjectileIndex, int projectileType, int targetNPCIndex, Point[] bufferForScan) {
        if (protectedProjectileIndex >= 0 && protectedProjectileIndex < Main.maxProjectiles) {
            Main.projectile[protectedProjectileIndex].netUpdate = true;
            Main.projectile[protectedProjectileIndex].GetGlobalProjectile<ProjectileItemData>().NoSpecialEffects = true;
        }
        orig(protectedProjectileIndex, projectileType, targetNPCIndex, bufferForScan);
    }

    void ILoad.Load(Mod mod) {
        On_Projectile.KillOldestJavelin += On_Projectile_KillOldestJavelin;
    }

    void ILoad.Unload() { }
}