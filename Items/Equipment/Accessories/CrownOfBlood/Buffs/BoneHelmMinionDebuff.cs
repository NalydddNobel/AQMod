using Aequus.Common.NPCs.Global;
using Aequus.Items.Equipment.Accessories.CrownOfBlood.Buffs;

namespace Aequus.Items.Equipment.Accessories.CrownOfBlood.Buffs {
    public class BoneHelmMinionDebuff : ModBuff {
        public static int Damage = 60;
        public static int DamageNumber = 12;

        public override string Texture => AequusTextures.Debuff.FullPath;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.Aequus().debuffBoneHelmEmpowered = true;
            if (npc.TryGetGlobalNPC(out StatSpeedGlobalNPC speedNPC)) {
                speedNPC.statSpeed *= 0.5f;
            }
        }
    }
}

namespace Aequus {
    public partial class AequusNPC {
        public bool debuffBoneHelmEmpowered;

        private void DebuffEffect_BoneHelmEmpowered(NPC npc) {
            if (!debuffBoneHelmEmpowered) {
                return;
            }
        }

        private void UpdateLifeRegen_BoneHelmEmpowered(NPC npc, ref LifeRegenModifiers modifiers) {
            if (debuffBoneHelmEmpowered) {
                modifiers.LifeRegen -= BoneHelmMinionDebuff.Damage;
                modifiers.DamageNumber = BoneHelmMinionDebuff.DamageNumber;
            }
        }
    }
}