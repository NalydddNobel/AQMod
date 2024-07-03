using ReLogic.Content;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;

namespace Aequu2.Old.Content.Bosses;

public class LegacyAequu2BossSupportedBar : ModBossBar {
    private int _headIndex;

    public override Asset<Texture2D> GetIconTexture(ref Rectangle? iconFrame) {
        return TextureAssets.NpcHeadBoss[_headIndex];
    }

    public override bool? ModifyInfo(ref BigProgressBarInfo info, ref float life, ref float lifeMax, ref float shield, ref float shieldMax) {
        NPC npc = Main.npc[info.npcIndexToAimAt];
        if (!npc.active) {
            return false;
        }

        life = npc.life;
        lifeMax = npc.lifeMax;

        int headIndex = npc.GetBossHeadTextureIndex();
        if (headIndex == -1) {
            return false;
        }

        _headIndex = headIndex;
        // Dissapear when in negative states.
        return npc.ai[0] >= 0f;
    }
}
