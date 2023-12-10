using System.Collections.Generic;
using Terraria.ModLoader;

namespace Aequus.Core.Reflection;
public class MethodListWatcher : ModSystem {
    internal static List<IMethodList> _methodListsToLoad = new();

    public override void PostSetupContent() {
        foreach (var m in _methodListsToLoad) {
            m.Init();
        }
    }
}