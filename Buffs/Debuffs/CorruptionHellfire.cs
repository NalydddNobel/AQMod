using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Buffs.Debuffs
{
    public class CorruptionHellfire : ModBuff
    {
        public static Color FireColor => new Color(75, 20, 120, 10);

        public static void Inflict(NPC npc, int time)
        {
            int buffID = ModContent.BuffType<CorruptionHellfire>();
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
            npc.GetGlobalNPC<AQNPC>().corruptHellfire = true;
        }
    }
}
