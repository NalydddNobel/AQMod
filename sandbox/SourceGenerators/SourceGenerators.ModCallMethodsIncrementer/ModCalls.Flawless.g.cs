namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool Flawless(object[] args) {
        if (args[1] is not Terraria.NPC npc) {
            LogError($"Mod Call Parameter index 1 (\"npc\") did not match Type \"Terraria.NPC\".");
            return default;
        }

        if (args[2] is not Terraria.Player player) {
            LogError($"Mod Call Parameter index 2 (\"player\") did not match Type \"Terraria.Player\".");
            return default;
        }

        return global::Aequus.Common.NPCs.Global.FlawlessGlobalNPC.GetPlayerFlawlessFlag(npc, player);
    }
}