using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class LocustDebuff : ModBuff
    {
        public override string Texture => Aequus.PlaceholderDebuff;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.PlayerDoTBuff.Add(Type);
        }

        public static void AddStack(NPC npc, int debuffTime, int stacksAmt = 1)
        {
            npc.AddBuff(ModContent.BuffType<LocustDebuff>(), debuffTime);
            npc.Aequus().locustStacks += (byte)stacksAmt;
            npc.netUpdate = true;
        }
    }
}