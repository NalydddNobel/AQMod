using Aequus.NPCs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class LocustDebuff : ModBuff
    {
        public override string Texture => Aequus.Debuff;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public static void AddStack(NPC npc, int debuffTime, int stacksAmt = 1)
        {
            npc.AddBuff(ModContent.BuffType<LocustDebuff>(), debuffTime);
            npc.Aequus().locustStacks += (byte)stacksAmt;
            npc.netUpdate = true;
            if (Main.netMode != NetmodeID.SinglePlayer)
            {
                AequusNPC.Sync(npc.whoAmI);
            }
        }
    }
}