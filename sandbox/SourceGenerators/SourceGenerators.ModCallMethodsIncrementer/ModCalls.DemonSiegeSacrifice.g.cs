namespace Aequus;

public partial class ModCallHandler {
    static bool DemonSiegeSacrifice(object[] args) {
        if (args[1] is not int fromItem) {
            throw new System.Exception($"Mod Call Parameter index 1 (\"fromItem\") did not match Type \"int\".");
        }

        int itemTo = default;
        if (args.Length > 2) {
            if (args[2] is int optionalValue) {
                itemTo = optionalValue;
            }
            else {
                LogError($"Optional Mod Call Parameter index 2 (\"itemTo\") did not match Type \"int\".");
            }
        }

        int progression = default;
        if (args.Length > 3) {
            if (args[3] is int optionalValue) {
                progression = optionalValue;
            }
            else {
                LogError($"Optional Mod Call Parameter index 3 (\"progression\") did not match Type \"int\".");
            }
        }

        return global::Aequus.Content.Events.DemonSiege.DemonSiegeSystem.CallAddDemonSiegeData(fromItem, itemTo, progression);
    }
}