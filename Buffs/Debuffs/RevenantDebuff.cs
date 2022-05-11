using Aequus.NPCs;
using Terraria;

namespace Aequus.Buffs.Debuffs
{
    public class RevenantDebuff : NecromancyDebuff
    {
        public override string Texture => AequusHelpers.GetPath<NecromancyDebuff>();

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<NecromancyNPC>().zombieDrain = 3 * AequusHelpers.NPCREGEN;
        }
    }
}