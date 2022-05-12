using Aequus.NPCs;
using Terraria;

namespace Aequus.Buffs.Debuffs
{
    public class RevenantDebuff : NecromancyDebuff
    {
        public override string Texture => AequusHelpers.GetPath<NecromancyDebuff>();

        public override void Update(NPC npc, ref int buffIndex)
        {
            int damageOverTime = 3;
            if (npc.life < 50)
            {
                damageOverTime = 50;
            }
            else if (npc.life < 100)
            {
                damageOverTime = 6;
            }
            npc.GetGlobalNPC<NecromancyNPC>().zombieDrain = damageOverTime * AequusHelpers.NPCREGEN;
        }
    }
}