namespace Aequus;

public partial class ModCallHandler {
    static bool xmasHats(object[] args) {
        if (args.Length > 1) {
            if (args[1] is not bool value) {
                throw new System.Exception($"Mod Call Parameter index 1 (\"value\") did not match Type \"bool\".");
            }

            global::Aequus.AequusWorld.xmasHats = value;
        }

        return global::Aequus.AequusWorld.xmasHats;
    }
}