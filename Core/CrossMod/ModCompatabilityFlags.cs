namespace Aequus.Core.CrossMod;

internal class ModCompatabilityFlags : ILoadable {
    public static bool RemoveExpertExclusivity { get; set; }

    public void Load(Mod mod) {
    }

    public void Unload() {
        RemoveExpertExclusivity = false;
    }
}