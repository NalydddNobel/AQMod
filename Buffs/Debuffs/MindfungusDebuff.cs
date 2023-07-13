using Aequus.Buffs.Debuffs;
using Aequus.Common.Buffs;
using Aequus.Common.DataSets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs {
    public class MindfungusDebuff : ModBuff {
        public static int BaseDamage = 16;
        public static int StackDamage = 8;
        public static int BaseDamageNumber = 5;
        public static int StackDamageNumber = 1;

        public override string Texture => Aequus.VanillaTexture + "Buff_" + BuffID.Bleeding;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            foreach (var n in ContentSamples.NpcsByNetId) {
                if (n.Value.boss || n.Value.defense >= 100) {
                    AequusBuff.SetImmune(n.Key, false, Type);
                }
            }
            BuffSets.PlayerDoTDebuff.Add(Type);
        }

        public override void Update(NPC npc, ref int buffIndex) {
            npc.Aequus().debuffMindfungus = true;
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public bool debuffMindfungus;
        public byte mindfungusStacks;

        private void UpdateLifeRegen_Mindfungus(NPC npc, AequusNPC aequus, ref LifeRegenModifiers modifiers) {
            if (debuffCorruptionFire) {
                modifiers.LifeRegen -= MindfungusDebuff.BaseDamage + MindfungusDebuff.StackDamage * aequus.mindfungusStacks;
                modifiers.DamageNumber = MindfungusDebuff.BaseDamageNumber + MindfungusDebuff.StackDamageNumber * aequus.mindfungusStacks;
            }
            else {
                aequus.corruptionHellfireStacks = 0;
            }
        }
    }
}