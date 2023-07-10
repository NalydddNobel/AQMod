using Terraria;

namespace Aequus.NPCs {
    public partial class AequusNPC {
        private void ModifyHit(NPC npc, Player player, ref NPC.HitModifiers modifiers) {
            ModifyHit_ProcMeathook(npc, ref modifiers);
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers) {
            ModifyHit(npc, player, ref modifiers);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
            ModifyHit(npc, Main.player[projectile.owner], ref modifiers);
            if (projectile.IsMinionOrSentryRelated) {
                ModifyHit_ProcFlowerCrownTag(npc, ref modifiers);
            }
        }

        private void OnHitBy(NPC npc, Player player, NPC.HitInfo hit) {
            OnHit_PlayMeathookSound(npc);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone) {
            OnHitBy(npc, player, hit);
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone) {
            OnHitBy(npc, Main.player[projectile.owner], hit);
            if (projectile.IsMinionOrSentryRelated) {
                OnHit_FlowerCrownEffect(npc);
            }
        }
    }
}