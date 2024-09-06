namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool DemonSiegeSacrificeHide(object[] args) {
        if (args[1] is not int fromItem) {
            LogError($"Mod Call Parameter index 1 (\"fromItem\") did not match Type \"int\".");
            return default;
        }

        if (args[2] is not bool hide) {
            LogError($"Mod Call Parameter index 2 (\"hide\") did not match Type \"bool\".");
            return default;
        }

        return global::Aequus.Content.Events.DemonSiege.DemonSiegeSystem.CallHideDemonSiegeData(fromItem, hide);
    }
}