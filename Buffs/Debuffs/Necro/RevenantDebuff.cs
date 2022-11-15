using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Terraria;

namespace Aequus.Buffs.Debuffs.Necro
{
    public class RevenantDebuff : NecromancyDebuff
    {
        public override string Texture => Aequus.Debuff;
        public override float Tier => 2f;
        public override int DamageSet => 40;
        public override float BaseSpeed => 1f;

        public override void Update(NPC npc, ref int buffIndex)
        {
            int damageOverTime = 24;
            if (npc.life < 50)
            {
                damageOverTime = 400;
            }
            else if (npc.life < 100)
            {
                damageOverTime = 48;
            }
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            zombie.ghostDebuffDOT = damageOverTime;
            zombie.ghostDamage = DamageSet;
            zombie.ghostSpeed = BaseSpeed;
            zombie.DebuffTier(Tier);
            zombie.RenderLayer(ColorTargetID.Revenant);
        }
    }
}