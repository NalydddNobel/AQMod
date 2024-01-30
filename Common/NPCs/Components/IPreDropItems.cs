namespace Aequus.Common.NPCs.Components;
public interface IPreDropItems {
    bool PreDropItems(Player closestPlayer);

    internal static void On_NPC_NPCLoot_DropItems(On_NPC.orig_NPCLoot_DropItems orig, NPC self, Player closestPlayer) {
        if (self.ModNPC is IPreDropItems preDropItems && !preDropItems.PreDropItems(closestPlayer)) {
            return;
        }
        orig(self, closestPlayer);
    }
}