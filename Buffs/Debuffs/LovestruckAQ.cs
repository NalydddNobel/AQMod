using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class LovestruckAQ : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<AQNPC>().lovestruckAQ = true;
        }

        public static void Apply(NPC npc, int time, bool quiet = false)
        {
            npc.AddBuff(ModContent.BuffType<LovestruckAQ>(), time, quiet);
            npc.AddBuff(BuffID.Lovestruck, time, quiet);
        }
    }
}