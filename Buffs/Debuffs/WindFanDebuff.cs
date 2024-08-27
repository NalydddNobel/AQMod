using Aequus.Buffs.Debuffs;

namespace Aequus.Buffs.Debuffs {
    public class WindFanDebuff : ModBuff {
        public static int Damage = 25;
        public static int DamageNumber = 5;

        public override string Texture => Aequus.PlaceholderDebuff;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.Aequus().debuffWindFan = true;
        }
    }
}

namespace Aequus {
    partial class AequusNPC {
        public bool debuffWindFan;

        private void UpdateLifeRegen_WindFanDebuff(NPC npc, ref LifeRegenModifiers modifiers) {
            if (debuffWindFan) {
                modifiers.LifeRegen -= WindFanDebuff.Damage;
                modifiers.DamageNumber = WindFanDebuff.DamageNumber;
            }
        }
    }
}