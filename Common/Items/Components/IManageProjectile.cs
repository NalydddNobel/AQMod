namespace Aequus.Common.Items.Components;

/// <summary>
/// Allows the item to grant various effects to fired projectiles. All of these methods are not instanced, do NOT use <see cref="ModItem.Item"/> data.
/// The methods granted by this interface are:
/// <list type="bullet">
/// <item><see cref="PreAIProjectile(Projectile)"/></item>
/// <item><see cref="AIProjectile(Projectile)"/></item>
/// <item><see cref="PostAIProjectile(Projectile)"/></item>
/// <item><see cref="PreDrawProjectile(Projectile, ref Color)"/></item>
/// <item><see cref="PostDrawProjectile(Projectile, in Color)"/></item>
/// <item><see cref="OnHitNPCProjectile(Projectile, NPC, NPC.HitInfo, System.Int32)"/></item>
/// <item><see cref="OnTileCollideProjectile(Projectile, Vector2)"/></item>
/// <item><see cref="PreKillProjectile(Projectile, System.Int32)"/></item>
/// <item><see cref="OnKillProjectile(Projectile, System.Int32)"/></item>
/// </list>
/// </summary>
public interface IManageProjectile {
    System.Boolean PreAIProjectile(Projectile projectile) {
        return true;
    }
    void AIProjectile(Projectile projectile) {
    }
    void PostAIProjectile(Projectile projectile) {
    }

    System.Boolean PreDrawProjectile(Projectile projectile, ref Color lightColor) {
        return true;
    }
    void PostDrawProjectile(Projectile projectile, in Color lightColor) {
    }

    void OnHitNPCProjectile(Projectile projectile, NPC target, NPC.HitInfo hit, System.Int32 damageDone) {
    }

    System.Boolean OnTileCollideProjectile(Projectile projectile, Vector2 oldVelocity) {
        return true;
    }

    System.Boolean PreKillProjectile(Projectile projectile, System.Int32 timeLeft) {
        return true;
    }
    void OnKillProjectile(Projectile projectile, System.Int32 timeLeft) {
    }
}