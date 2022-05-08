using Aequus.NPCs;
using Terraria;

namespace Aequus.Buffs.Debuffs
{
    public class InsurgentDebuff : NecromancyDebuff
    {
        public override string Texture => AequusHelpers.GetPath<NecromancyDebuff>();

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NecromancyNPC>().zombieDrain = 20 * AequusHelpers.NPCREGEN;
        }
    }
}