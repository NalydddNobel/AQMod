namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool BlacklistSettleDownMsg(object[] args) {
        if (args[1] is not int npcId) {
            LogError($"Mod Call Parameter index 1 (\"npcId\") did not match Type \"int\".");
            return default;
        }

        if (args[2] is not bool value) {
            LogError($"Mod Call Parameter index 2 (\"value\") did not match Type \"bool\".");
            return default;
        }

        return global::Aequus.Content.Villagers.NPCSettleDownMessage.SetBlacklist(npcId, value);
    }
}