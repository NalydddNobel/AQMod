namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool BlacklistSettleDownMsg(object[] args) {
        if (args[1] is not int npcId) {
            throw new System.Exception($"Mod Call Parameter index 1 (\"npcId\") did not match Type \"int\".");
        }

        if (args[2] is not bool value) {
            throw new System.Exception($"Mod Call Parameter index 2 (\"value\") did not match Type \"bool\".");
        }

        return global::Aequus.Content.Villagers.NPCSettleDownMessage.SetBlacklist(npcId, value);
    }
}