namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool FlawlessCheck(object[] args) {
        if (args[1] is not Terraria.NPC npc) {
            LogError($"Mod Call Parameter index 1 (\"npc\") did not match Type \"Terraria.NPC\".");
            return default;
        }

        return global::Aequus.Common.NPCs.Global.FlawlessGlobalNPC.GetNoHitCheckFlag(npc);
    }
}