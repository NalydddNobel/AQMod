using Aequus.Old.Content.Necromancy;
using Aequus.Old.Content.Necromancy.Rendering;
using Aequus.Old.Content.Necromancy.Sceptres.Evil;
using System;

namespace Aequus.Old.CrossMod.AvalonSupport.Items;

public class BacciliteSceptreDebuff : NecromancyDebuff {
    public override string Texture => AequusTextures.TemporaryDebuffIcon;

    public override float Tier => 1f;
    public override int DamageSet => 40;
    public override float GhostSpeedBoost => 0.275f;

    public override string LocalizationCategory => "CrossMod.Avalon.Buffs";

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
        zombie.RenderLayer(ColorTargetID.BoogerGreen);
    }
}
