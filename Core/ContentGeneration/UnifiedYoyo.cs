using System;

namespace Aequu2.Core.ContentGeneration;

[Obsolete("Unfinished.")]
public abstract class UnifiedYoyo : ModProjectile {
    public override void SetDefaults() {
        Projectile.aiStyle = ProjAIStyleID.Yoyo;
        Projectile.friendly = true;
        Projectile.DamageType = DamageClass.MeleeNoSpeed;
        Projectile.penetrate = -1;
    }

    /// <summary>
    /// Determines whether this projectile is the orbiting yoyo from the Yoyo Glove.
    /// </summary>
    /// <returns></returns>
    public bool IsYoyoGloveYoyo() {
        for (int i = 0; i < Projectile.whoAmI; i++) {
            if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type) {
                return true;
            }
        }
        return false;
    }
}