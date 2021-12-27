using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class Shimmering : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<AQNPC>().shimmering = true;
        }
    }
}
