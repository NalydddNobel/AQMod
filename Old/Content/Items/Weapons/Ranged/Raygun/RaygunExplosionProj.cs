using Aequu2.Core.Entities.Projectiles;

namespace Aequu2.Old.Content.Items.Weapons.Ranged.Raygun;

public class RaygunExplosionProj : ModProjectile {
    public override string Texture => Aequu2Textures.None.Path;

    public override void SetDefaults() {
        Projectile.width = 120;
        Projectile.height = 120;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 4;
        Projectile.usesIDStaticNPCImmunity = true;
        Projectile.idStaticNPCHitCooldown = Projectile.timeLeft + 2;
        Projectile.penetrate = -1;
        Projectile.GetGlobalProjectile<ProjectileItemData>().NoSpecialEffects = true;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        Projectile.damage = (int)(Projectile.damage * 0.75f);
    }
}
