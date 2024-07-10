namespace AequusRemake.Systems.CrossMod;

internal class ModCompatabilityFlags : ILoad {
    public static bool RemoveExpertExclusivity { get; set; }

    public void Load(Mod mod) {
    }

    public void Unload() {
        RemoveExpertExclusivity = false;
    }
}