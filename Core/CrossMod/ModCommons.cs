using Aequus.Content.CrossMod;

namespace Aequus.Core.CrossMod;

internal class ModCommons : ILoadable {
    public static bool ExpertDropsInClassicMode { get; set; }

    public void Load(Mod mod) {
    }

    public void Unload() {
        ExpertDropsInClassicMode = false;
    }
}