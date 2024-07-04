namespace AequusRemake.Core.Entities.Items.Components;

/// <summary>
/// Allows the item to grant various effects to fired projectiles. All of these methods are not instanced, do NOT use <see cref="ModItem.Item"/> data.
/// The methods granted by this interface are:
/// <list type="bullet">
/// <item><see cref="PreAIProjectile(Projectile)"/></item>
/// <item><see cref="AIProjectile(Projectile)"/></item>
/// <item><see cref="PostAIProjectile(Projectile)"/></item>
/// <item><see cref="PreDrawProjectile(Projectile, ref Color)"/></item>
/// <item><see cref="PostDrawProjectile(Projectile, in Color)"/></item>
/// <item><see cref="OnHitNPCProjectile(Projectile, NPC, NPC.HitInfo, int)"/></item>
/// <item><see cref="OnTileCollideProjectile(Projectile, Vector2)"/></item>
/// <item><see cref="PreKillProjectile(Projectile, int)"/></item>
/// <item><see cref="OnKillProjectile(Projectile, int)"/></item>
/// </list>
/// </summary>
public interface IManageProjectile {
    bool PreAIProjectile(Projectile projectile) {
        return true;
    }
    void AIProjectile(Projectile projectile) {
    }
    void PostAIProjectile(Projectile projectile) {
    }

    bool PreDrawProjectile(Projectile projectile, ref Color lightColor) {
        return true;
    }
    void PostDrawProjectile(Projectile projectile, in Color lightColor) {
    }

    void OnHitNPCProjectile(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
    }

    bool OnTileCollideProjectile(Projectile projectile, Vector2 oldVelocity) {
        return true;
    }

    bool PreKillProjectile(Projectile projectile, int timeLeft) {
        return true;
    }
    void OnKillProjectile(Projectile projectile, int timeLeft) {
    }
}