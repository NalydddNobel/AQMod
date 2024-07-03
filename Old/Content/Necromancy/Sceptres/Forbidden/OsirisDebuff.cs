using Aequu2.Old.Content.Necromancy.Rendering;
using Aequu2.Old.Content.Necromancy.Sceptres.Evil;

namespace Aequu2.Old.Content.Necromancy.Sceptres.Forbidden;

public class OsirisDebuff : NecromancyDebuff {
    public override string Texture => Aequu2Textures.TemporaryDebuffIcon;

    public override float Tier => 3f;
    public override int DamageSet => 75;
    public override float GhostSpeedBoost => 1.25f;

    public override void Update(NPC npc, ref int buffIndex) {
        var zombie = npc.GetGlobalNPC<NecromancyNPC>();
        zombie.ghostDebuffDOT = 40;
        zombie.ghostDamage = DamageSet;
        zombie.ghostSpeed = GhostSpeedBoost;
        zombie.DebuffTier(Tier);
        zombie.RenderLayer(ColorTargetID.Osiris);
    }
}