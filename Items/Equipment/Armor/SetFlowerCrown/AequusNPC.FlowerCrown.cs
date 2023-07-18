using Aequus.Items.Equipment.Armor.SetFlowerCrown;
using Terraria;
using Terraria.ID;

namespace Aequus.NPCs {
    public partial class AequusNPC {
        private void ModifyHit_ProcFlowerCrownTag(NPC npc, ref NPC.HitModifiers modifiers) {
            if (npc.HasBuff<FlowerCrownWhipTagDebuff>()) {
                modifiers.FlatBonusDamage += FlowerCrown.TagDamage;
            }
        }

        private void OnHit_FlowerCrownEffect(NPC npc) {
            if (npc.HasBuff<FlowerCrownWhipTagDebuff>()) {
                for (int i = 0; i < 5; i++) {
                    var d = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Confetti_Yellow, Scale: 0.7f);
                    d.fadeIn = d.scale + 0.5f;
                    d.noGravity = true;
                    d.velocity *= 2f;
                }
            }
        }
    }
}