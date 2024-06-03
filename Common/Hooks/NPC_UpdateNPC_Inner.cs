#if !DEBUG
using Aequus.Old.Content.Items.Weapons.Magic.Gamestar;
#endif

namespace Aequus.Common.Hooks;

public partial class TerrariaHooks {
    private static void NPC_UpdateNPC_Inner(On_NPC.orig_UpdateNPC_Inner orig, NPC npc, int i) {
#if !DEBUG
        if (!GamestarDebuff._loop && npc.TryGetGlobalNPC(out GamestarNPC gamestar) && gamestar.gamestarDebuffMaxTime > 0) {
            if (gamestar.gamestarDebuffTimer >= gamestar.gamestarDebuffMaxTime) {
                gamestar.UpdateLaggyNPC(npc, i);
                gamestar.gamestarDebuffTimer = 0;
            }
            else {
                NPC_UpdateNPC_Frozen(npc, lowerBuffTime: true);
            }
            return;
        }
#endif

        orig(npc, i);
    }

    private static void NPC_UpdateNPC_Frozen(NPC npc, bool lowerBuffTime = true) {
        npc.lifeRegen = 0;
        npc.lifeRegenExpectedLossPerSecond = -1;
        NPCLoader.ResetEffects(npc);
        npc.UpdateNPC_BuffSetFlags(lowerBuffTime: lowerBuffTime);
    }
}
