using Aequus.Content.Necromancy.Rendering;
using System;

namespace Aequus.Content.Necromancy.Sceptres.Evil;

public class CrimsonSceptreDebuff : NecromancyDebuff {
    public override string Texture => AequusTextures.Debuff.Path;

    public override float Tier => 1f;
    public override int DamageSet => 20;
    public override float GhostSpeedBoost => 0.3f;

    public override void Update(NPC npc, ref int buffIndex) {
        int damageOverTime = 24;
        int killMeLife = Math.Min(npc.lifeMax / 10, 50);
        if (npc.life < killMeLife) {
            damageOverTime = 400;
        }
        else if (npc.life < killMeLife * 2) {
            damageOverTime = 48;
        }
        var zombie = npc.GetGlobalNPC<NecromancyNPC>();
        zombie.ghostDebuffDOT = damageOverTime;
        zombie.ghostDamage = DamageSet;
        zombie.ghostSpeed = GhostSpeedBoost;
        zombie.DebuffTier(Tier);
        zombie.RenderLayer(ColorTargetID.CrimsonSceptre);
    }
}
