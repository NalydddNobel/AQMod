using Aequus.Common.Items.Components;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Common.Projectiles;

public partial class AequusProjectile : GlobalProjectile {
    protected override bool CloneNewInstances => true;
    public override bool InstancePerEntity => true;

    /// <summary>
    /// Custom data to be used by items which utilize the <see cref="IManageProjectile"/> interface. This data is synced in multiplayer.
    /// </summary>
    public int itemData;

    /// <summary>
    /// If this is true, special effects should be disabled on the projectile. This is only set to true by Javelin-like projectiles.
    /// </summary>
    public bool noSpecialEffects;

    private IManageProjectile _projectileManager;

    public bool IsChildOrNoSpecialEffects => isProjectileChild || noSpecialEffects;
    public bool HasNPCOwner => parentNPCIndex != -1;

    public override void Load() {
        Load_JavelinFixes();
    }

    public override void SetDefaults(Projectile projectile) {
        noSpecialEffects = false;
        itemData = 0;
        parentItemType = 0;
        parentNPCIndex = -1;
        _projectileManager = default;
    }

    public override bool PreAI(Projectile projectile) {
        PreAI_UpdateSources(projectile);
        return (_projectileManager?.PreAIProjectile(projectile, this)).GetValueOrDefault(true);
    }

    public override void AI(Projectile projectile) {
        _projectileManager?.AIProjectile(projectile, this);
    }

    public override void PostAI(Projectile projectile) {
        _projectileManager?.PostAIProjectile(projectile, this);
    }

    public override bool PreDraw(Projectile projectile, ref Color lightColor) {
        return (_projectileManager?.PreDrawProjectile(projectile, this, ref lightColor)).GetValueOrDefault(true);
    }

    public override void PostDraw(Projectile projectile, Color lightColor) {
        _projectileManager?.PostDrawProjectile(projectile, this, lightColor);
    }

    public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity) {
        return (_projectileManager?.OnTileCollideProjectile(projectile, this, oldVelocity)).GetValueOrDefault(true);
    }

    public override bool PreKill(Projectile projectile, int timeLeft) {
        return (_projectileManager?.PreKillProjectile(projectile, this, timeLeft)).GetValueOrDefault(true);
    }

    public override void OnKill(Projectile projectile, int timeLeft) {
        _projectileManager?.OnKillProjectile(projectile, this, timeLeft);
    }

    public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
        _projectileManager?.OnHitNPCProjectile(projectile, this, target, hit, damageDone);
    }
}