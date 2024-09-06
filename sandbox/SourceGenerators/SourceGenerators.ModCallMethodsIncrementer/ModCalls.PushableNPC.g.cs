namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool PushableNPC(object[] args) {
        if (args[1] is not int proj) {
            LogError($"Mod Call Parameter index 1 (\"proj\") did not match Type \"int\".");
            return default;
        }

        bool? setValue = default;
        if (args.Length > 2) {
            if (args[2] is bool optionalValue) {
                setValue = optionalValue;
            }
            else {
                LogError($"Optional Mod Call Parameter index 2 (\"setValue\") did not match Type \"bool\".");
            }
        }

        return global::Aequus.Content.LegacyPushableEntities.IsNPCPushable(proj, setValue);
    }
}