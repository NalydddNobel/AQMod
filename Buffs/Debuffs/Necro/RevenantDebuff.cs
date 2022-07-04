using Aequus.Content.Necromancy;
using Terraria;

namespace Aequus.Buffs.Debuffs.Necro
{
    public class RevenantDebuff : NecromancyDebuff
    {
        public override string Texture => Aequus.Debuff;
        public override float Tier => 2f;

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
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            zombie.zombieDrain = damageOverTime * AequusHelpers.NPCREGEN;
            zombie.DebuffTier(Tier);
            zombie.RenderLayer(GhostOutlineRenderer.IDs.Revenant);
        }
    }
}