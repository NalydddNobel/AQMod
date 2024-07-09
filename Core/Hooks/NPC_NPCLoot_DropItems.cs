using AequusRemake.Systems.NPCs;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    internal static void On_NPC_NPCLoot_DropItems(On_NPC.orig_NPCLoot_DropItems orig, NPC self, Player closestPlayer) {
        if (self.ModNPC is IPreDropItems preDropItems && !preDropItems.PreDropItems(closestPlayer)) {
            return;
        }

        orig(self, closestPlayer);
    }
}
