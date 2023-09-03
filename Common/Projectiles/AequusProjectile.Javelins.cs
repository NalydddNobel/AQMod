using Microsoft.Xna.Framework;
using Terraria;

namespace Aequus;

public partial class AequusProjectile {
    private void Load_JavelinFixes() {
        On_Projectile.KillOldestJavelin += On_Projectile_KillOldestJavelin;
    }

    private static void On_Projectile_KillOldestJavelin(On_Projectile.orig_KillOldestJavelin orig, int protectedProjectileIndex, int projectileType, int targetNPCIndex, Point[] bufferForScan) {
        if (protectedProjectileIndex >= 0 && protectedProjectileIndex < Main.maxProjectiles) {
            Main.projectile[protectedProjectileIndex].netUpdate = true;
            Main.projectile[protectedProjectileIndex].GetGlobalProjectile<AequusProjectile>().noSpecialEffects = true;
        }
        orig(protectedProjectileIndex, projectileType, targetNPCIndex, bufferForScan);
    }
}