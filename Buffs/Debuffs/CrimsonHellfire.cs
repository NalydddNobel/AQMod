using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class CrimsonHellfire : ModBuff
    {
        public static Color FireColor => new Color(175, 65, 20, 10);

        public static void Inflict(NPC npc, int time)
        {
            int buffID = ModContent.BuffType<CrimsonHellfire>();
            if (npc.buffImmune[buffID])
            {
                npc.AddBuff(BuffID.OnFire, time);
            }
            else
            {
                npc.AddBuff(buffID, time);
            }
        }

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
