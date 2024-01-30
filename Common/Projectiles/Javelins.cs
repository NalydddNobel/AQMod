namespace Aequus.Common.Projectiles;

public class Javelins : ILoadable {
    private static void On_Projectile_KillOldestJavelin(On_Projectile.orig_KillOldestJavelin orig, System.Int32 protectedProjectileIndex, System.Int32 projectileType, System.Int32 targetNPCIndex, Point[] bufferForScan) {
        if (protectedProjectileIndex >= 0 && protectedProjectileIndex < Main.maxProjectiles) {
            Main.projectile[protectedProjectileIndex].netUpdate = true;
            Main.projectile[protectedProjectileIndex].GetGlobalProjectile<ProjectileItemData>().NoSpecialEffects = true;
        }
        orig(protectedProjectileIndex, projectileType, targetNPCIndex, bufferForScan);
    }

    void ILoadable.Load(Mod mod) {
        On_Projectile.KillOldestJavelin += On_Projectile_KillOldestJavelin;
    }

    void ILoadable.Unload() { }
}