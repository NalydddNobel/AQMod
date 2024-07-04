namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static void NPC_NPCLoot_DropMoney(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player closestPlayer) {
        orig(self, closestPlayer);
    }
}
