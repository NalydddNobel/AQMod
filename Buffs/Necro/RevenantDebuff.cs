using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using System;
using Terraria;

namespace Aequus.Buffs.Necro
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
            int killMeLife = Math.Min(npc.lifeMax / 10, 50);
            if (npc.life < killMeLife)
            {
                damageOverTime = 400;
            }
            else if (npc.life < killMeLife * 2)
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