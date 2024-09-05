namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool usedWhiteFlag(object[] args) {
        if (args.Length > 1) {
            if (args[1] is not bool value) {
                throw new System.Exception($"Mod Call Parameter index 1 (\"value\") did not match Type \"bool\".");
            }

            global::Aequus.AequusWorld.usedWhiteFlag = value;
        }

        return global::Aequus.AequusWorld.usedWhiteFlag;
    }
}