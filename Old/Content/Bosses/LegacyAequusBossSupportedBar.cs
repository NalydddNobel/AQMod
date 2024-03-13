using Terraria.GameContent.UI.BigProgressBar;

namespace Aequus.Old.Content.Bosses;

public class LegacyAequusBossSupportedBar : ModBossBar {
    public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax) {
        NPC npc = Main.npc[info.npcIndexToAimAt];
        if (!npc.active) {
            return false;
        }

        life = npc.life;
        lifeMax = npc.lifeMax;

        // Dissapear when in negative states.
        return npc.ai[0] >= 0f;
    }
}
