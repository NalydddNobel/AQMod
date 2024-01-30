namespace Aequus.Core.CrossMod;

internal class ModCompatabilityFlags : ILoadable {
    public static System.Boolean RemoveExpertExclusivity { get; set; }

    public void Load(Mod mod) {
    }

    public void Unload() {
        RemoveExpertExclusivity = false;
    }
}