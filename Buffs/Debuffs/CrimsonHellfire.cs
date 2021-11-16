using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class CrimsonHellfire : ModBuff
    {
        public static Color FireColor => new Color(170, 40, 10, 10);

        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<AQNPC>().crimsonHellfire = true;
        }
    }
}
