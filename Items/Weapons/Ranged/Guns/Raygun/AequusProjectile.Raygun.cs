using Aequus.Items.Weapons.Ranged.Guns.Raygun;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Projectiles;

public partial class AequusProjectile : GlobalProjectile {
    public void AI_Raygun(Projectile projectile) {
        if (sourceItemUsed == ModContent.ItemType<Raygun>()) {
            if (Main.myPlayer == projectile.owner && projectile.numUpdates == -1 && projectile.velocity.Length() > 1f) {
                int p = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Vector2.Normalize(projectile.velocity) * 0.01f,
                    ModContent.ProjectileType<RaygunTrailProj>(), 0, 0f, projectile.owner);
                Main.projectile[p].rotation = projectile.rotation;
                Main.projectile[p].netUpdate = true;
                Main.projectile[p].ModProjectile<RaygunTrailProj>().color = Raygun.GetColor(projectile).UseA(0);
            }
            if (projectile.type == ProjectileID.ChlorophyteBullet) {
                projectile.alpha = 255;
            }
        }
    }

    public bool PreDraw_Raygun(Projectile projectile) {
        if (sourceItemUsed == ModContent.ItemType<Raygun>()) {
            if (!Raygun.BulletColor.ContainsKey(projectile.type)) {
                var clr = Raygun.CheckRayColor(projectile);
                Raygun.BulletColor[projectile.type] = (p) => p.GetAlpha(clr);
            }
            return projectile.velocity.Length() < 1f;
        }
        return true;
    }

    public void OnHit_Raygun(Projectile projectile, Entity victim) {
        if (sourceItemUsed == ModContent.ItemType<Raygun>()) {
            Raygun.SpawnExplosion(projectile.GetSource_OnHit(victim), projectile);
        }
    }

    public void Kill_Raygun(Projectile projectile) {
        if (sourceItemUsed == ModContent.ItemType<Raygun>()) {
            Raygun.SpawnExplosion(projectile.GetSource_Death(), projectile);
        }
    }
}