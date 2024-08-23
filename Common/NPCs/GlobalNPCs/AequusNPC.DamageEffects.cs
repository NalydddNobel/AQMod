namespace Aequus.NPCs;
public partial class AequusNPC {
    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
        if (projectile.IsMinionOrSentryRelated) {
            ModifyHit_ProcFlowerCrownTag(npc, ref modifiers);
        }
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
        if (projectile.IsMinionOrSentryRelated) {
            OnHit_FlowerCrownEffect(npc);
        }
    }
}