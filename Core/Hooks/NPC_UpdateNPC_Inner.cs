
namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_NPC_UpdateNPC_Inner(On_NPC.orig_UpdateNPC_Inner orig, NPC npc, int i) {
        bool frozen = false;
        if (frozen) {
            npc.lifeRegen = 0;
            npc.lifeRegenExpectedLossPerSecond = -1;
            NPCLoader.ResetEffects(npc);
            npc.UpdateNPC_BuffSetFlags(lowerBuffTime: true);
            return;
        }

        orig(npc, i);
    }
}
