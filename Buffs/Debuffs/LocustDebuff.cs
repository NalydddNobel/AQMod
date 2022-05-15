using Aequus.NPCs;
using Terraria;
using Terraria.ID;
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

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NPCDebuffs>().hasLocust = true;
        }

        public static void AddStack(NPC npc, int debuffTime, int stacksAmt = 1)
        {
            npc.AddBuff(ModContent.BuffType<LocustDebuff>(), debuffTime);
            npc.GetGlobalNPC<NPCDebuffs>().hasLocust = true;
            npc.GetGlobalNPC<NPCDebuffs>().locustStacks += (byte)stacksAmt;
            npc.netUpdate = true;
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                NPCDebuffs.SyncDebuffs(npc.whoAmI);
            }
        }
    }
}