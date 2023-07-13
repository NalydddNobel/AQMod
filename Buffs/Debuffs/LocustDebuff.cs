using Aequus.Buffs.Debuffs;
using Aequus.Common.Buffs;
using Aequus.Common.DataSets;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs {
    public class LocustDebuff : ModBuff, BuffHooks.IOnAddBuff {
        public static int BaseDamage = 2;
        public static int StackDamage = 2;
        public static byte MaxStacks = 200;
        public static int BaseDamageNumber = 1;
        public static int StackDamageNumber = 1;

        public override string Texture => Aequus.PlaceholderDebuff;

        public override void SetStaticDefaults() {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffSets.PlayerDoTDebuff.Add(Type);
        }

        public override bool ReApply(NPC npc, int time, int buffIndex) {
            var aequus = npc.Aequus();
            aequus.locustStacks = (byte)Math.Min(aequus.locustStacks + 1, MaxStacks);
            return false;
        }

        public override void Update(NPC npc, ref int buffIndex) {
            var aequus = npc.Aequus();
            aequus.locustStacks = Math.Max(aequus.locustStacks, (byte)1);
            aequus.debuffLocust = true;
        }
    }
}

namespace Aequus.NPCs {
    public partial class AequusNPC {
        public bool debuffLocust;
        public byte locustStacks;

        private void UpdateLifeRegen_Locusts(NPC npc, AequusNPC aequus, ref LifeRegenModifiers modifiers) {
            if (debuffLocust) {
                modifiers.LifeRegen -= LocustDebuff.BaseDamage + LocustDebuff.StackDamage * aequus.locustStacks;
                modifiers.DamageNumber = LocustDebuff.BaseDamageNumber + LocustDebuff.StackDamageNumber * aequus.locustStacks;
            }
            else {
                aequus.locustStacks = 0;
            }
        }
    }
}