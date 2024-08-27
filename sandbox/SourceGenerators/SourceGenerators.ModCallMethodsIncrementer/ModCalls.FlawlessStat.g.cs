namespace Aequus;

public partial class ModCallHandler {
    static bool[]? FlawlessStat(object[] args) {
        if (args[1] is not Terraria.NPC npc) {
            throw new System.Exception($"Mod Call Parameter index 1 (\"npc\") did not match Type \"Terraria.NPC\".");
        }

        return global::Aequus.Common.NPCs.Global.FlawlessGlobalNPC.GetPlayerFlawlessArray(npc);
    }
}