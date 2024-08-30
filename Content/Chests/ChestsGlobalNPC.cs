namespace Aequus.Content.Chests;

public class ChestsGlobalNPC : GlobalNPC {
    public override void OnKill(NPC npc) {
        if (npc.type == NPCID.WallofFlesh) {
            Instance<ChestUpgradeSystem>().OnHardmodeBossDefeat();
        }
    }
}
