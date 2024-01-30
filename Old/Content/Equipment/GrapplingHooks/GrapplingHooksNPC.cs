using Aequus.Old.Content.Equipment.GrapplingHooks.EnemyGrappleHook;
using Aequus.Old.Content.Equipment.GrapplingHooks.HealingGrappleHook;

namespace Aequus.Old.Content.Equipment.GrapplingHooks;

public class GrapplingHooksNPC : GlobalNPC {
    public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers) {
        Meathook.CheckMeathookDamage(npc, ref modifiers);
    }

    public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) {
        Meathook.CheckMeathookSound(npc);
        LeechHook.CheckLeechHook(player, npc);
    }

    public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
        Meathook.CheckMeathookSound(npc);
        if (projectile.TryGetOwner(out Player owner)) {
            LeechHook.CheckLeechHook(owner, npc);
        }
    }
}
