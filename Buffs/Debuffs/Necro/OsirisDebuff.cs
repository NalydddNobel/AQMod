using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Terraria;

namespace Aequus.Buffs.Debuffs.Necro
{
    public class OsirisDebuff : NecromancyDebuff
    {
        public override string Texture => AequusHelpers.GetPath<NecromancyDebuff>();

        public override void Update(NPC npc, ref int buffIndex)
        {
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            zombie.zombieDrain = 5 * AequusHelpers.NPCREGEN;

            if (zombie.renderLayer < GhostOutlineTarget.TargetIDs.FriendlyOsiris)
            {
                zombie.renderLayer = GhostOutlineTarget.TargetIDs.FriendlyOsiris;
            }
        }
    }
}