namespace Aequus;

public partial class ModCallHandler {
    [System.Runtime.CompilerServices.CompilerGenerated]
    static bool downedEventAtmosphere(object[] args) {
        if (args.Length > 1) {
            if (args[1] is not bool value) {
                throw new System.Exception($"Mod Call Parameter index 1 (\"value\") did not match Type \"bool\".");
            }

            global::Aequus.AequusWorld.downedEventAtmosphere = value;
        }

        return global::Aequus.AequusWorld.downedEventAtmosphere;
    }
}