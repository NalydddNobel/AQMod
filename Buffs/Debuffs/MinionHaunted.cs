using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public sealed class MinionHaunted : ModBuff
    {
        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            longerExpertDebuff = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<AQNPC>().minionHaunted = true;
        }
    }
}