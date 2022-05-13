using Aequus.NPCs;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class LocustDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public static void AddStack(NPC npc, int debuffTime, int amt = 1)
        {
            npc.AddBuff(ModContent.BuffType<LocustDebuff>(), debuffTime);
            npc.GetGlobalNPC<DamageOverTime>().hasLocust = true;
            npc.GetGlobalNPC<DamageOverTime>().locustStacks += amt;
            npc.netUpdate = true;
        }
    }
}