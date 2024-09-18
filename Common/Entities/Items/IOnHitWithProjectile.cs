namespace Aequus.Common.Entities.Items;

public interface IOnHitWithProjectile {
    void OnHitNPCWithProj(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers);
}
