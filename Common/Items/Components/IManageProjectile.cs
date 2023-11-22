using Aequus.Common.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Items.Components;

/// <summary>
/// Allows the item to grant various effects to fired projectiles. All of these methods are not instanced, do NOT use <see cref="ModItem.Item"/> data.
/// The methods granted by this interface are:
/// <list type="bullet">
/// <item><see cref="PreAIProjectile(Projectile, AequusProjectile)"/></item>
/// <item><see cref="AIProjectile(Projectile, AequusProjectile)"/></item>
/// <item><see cref="PostAIProjectile(Projectile, AequusProjectile)"/></item>
/// <item><see cref="PreDrawProjectile(Projectile, AequusProjectile, ref Color)"/></item>
/// <item><see cref="PostDrawProjectile(Projectile, AequusProjectile, in Color)"/></item>
/// <item><see cref="OnHitNPCProjectile(Projectile, AequusProjectile, NPC, NPC.HitInfo, int)"/></item>
/// <item><see cref="OnTileCollideProjectile(Projectile, AequusProjectile, Vector2)"/></item>
/// <item><see cref="PreKillProjectile(Projectile, AequusProjectile, int)"/></item>
/// <item><see cref="OnKillProjectile(Projectile, AequusProjectile, int)"/></item>
/// </list>
/// </summary>
public interface IManageProjectile {
    bool PreAIProjectile(Projectile projectile, AequusProjectile aequusProjectile) {
        return true;
    }
    void AIProjectile(Projectile projectile, AequusProjectile aequusProjectile) {
    }
    void PostAIProjectile(Projectile projectile, AequusProjectile aequusProjectile) {
    }

    bool PreDrawProjectile(Projectile projectile, AequusProjectile aequusProjectile, ref Color lightColor) {
        return true;
    }
    void PostDrawProjectile(Projectile projectile, AequusProjectile aequusProjectile, in Color lightColor) {
    }

    void OnHitNPCProjectile(Projectile projectile, AequusProjectile aequusProjectile, NPC target, NPC.HitInfo hit, int damageDone) {
    }

    bool OnTileCollideProjectile(Projectile projectile, AequusProjectile aequusProjectile, Vector2 oldVelocity) {
        return true;
    }

    bool PreKillProjectile(Projectile projectile, AequusProjectile aequusProjectile, int timeLeft) {
        return true;
    }
    void OnKillProjectile(Projectile projectile, AequusProjectile aequusProjectile, int timeLeft) {
    }
}