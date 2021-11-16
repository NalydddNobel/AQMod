using AQMod.Common;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class CorruptionHellfire : ModBuff
    {
        public static Color FireColor => new Color(120, 10, 100, 10);

        public override void SetDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<AQNPC>().corruptHellfire = true;
        }
    }
}
