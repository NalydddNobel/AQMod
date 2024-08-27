namespace Aequus;

public partial class ModCallHandler {
    static bool DemonSiegeSacrificeHide(object[] args) {
        if (args[1] is not int fromItem) {
            throw new System.Exception($"Mod Call Parameter index 1 (\"fromItem\") did not match Type \"int\".");
        }

        if (args[2] is not bool hide) {
            throw new System.Exception($"Mod Call Parameter index 2 (\"hide\") did not match Type \"bool\".");
        }

        return global::Aequus.Content.Events.DemonSiege.DemonSiegeSystem.CallHideDemonSiegeData(fromItem, hide);
    }
}