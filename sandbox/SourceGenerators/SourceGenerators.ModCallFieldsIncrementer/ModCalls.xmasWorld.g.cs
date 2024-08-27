namespace Aequus;

public partial class ModCallHandler {
    static bool xmasWorld(object[] args) {
        if (args.Length > 1) {
            if (args[1] is not bool value) {
                throw new System.Exception($"Mod Call Parameter index 1 (\"value\") did not match Type \"bool\".");
            }

            global::Aequus.AequusWorld.xmasWorld = value;
        }

        return global::Aequus.AequusWorld.xmasWorld;
    }
}