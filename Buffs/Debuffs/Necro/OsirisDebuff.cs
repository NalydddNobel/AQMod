using Aequus.Content.Necromancy;
using Aequus.Graphics;
using Terraria;

namespace Aequus.Buffs.Debuffs.Necro
{
    public class OsirisDebuff : NecromancyDebuff
    {
        public override string Texture => Aequus.Debuff;
        public override float Tier => 3f;

        public override void Update(NPC npc, ref int buffIndex)
        {
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            zombie.zombieDrain = 5 * AequusHelpers.NPCREGEN;
            zombie.DebuffTier(Tier);
            zombie.RenderLayer(GhostOutlineTarget.IDs.Osiris);
        }
    }
}