using Terraria.DataStructures;

namespace Aequus.Common.Golfing;

public class GolfBallGlobalProjectile : GlobalProjectile {
    public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) {
        return entity.ModProjectile is IGolfBallProjectile;
    }

    public override void OnSpawn(Projectile projectile, IEntitySource source) {
        if (projectile.damage != 0) {
            return;
        }

        for (int i = 0; i < Main.maxProjectiles; i++) {
            if (Main.projectile[i].active && (ProjectileID.Sets.IsAGolfBall[Main.projectile[i].type] || (Main.projectile[i].ModProjectile is IGolfBallProjectile && Main.projectile[i].damage == 0)) && i != projectile.whoAmI) {
                Main.projectile[i].Kill();
            }
        }
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor) {
        return true;
    }
}