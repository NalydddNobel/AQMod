using Aequus.Content.Necromancy;
using Aequus.Content.Necromancy.Renderer;
using Terraria;

namespace Aequus.Buffs.Debuffs.Necro
{
    public class OsirisDebuff : NecromancyDebuff
    {
        public override string Texture => Aequus.Debuff;
        public override float Tier => 3f;
        public override int DamageSet => 75;
        public override float BaseSpeed => 1.25f;

        public override void Update(NPC npc, ref int buffIndex)
        {
            var zombie = npc.GetGlobalNPC<NecromancyNPC>();
            zombie.ghostDebuffDOT = 40;
            zombie.ghostDamage = DamageSet;
            zombie.ghostSpeed = BaseSpeed;
            zombie.DebuffTier(Tier);
            zombie.RenderLayer(ColorTargetID.Osiris);
        }
    }
}