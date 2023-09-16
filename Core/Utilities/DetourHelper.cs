using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace Aequus.Core.Utilities;

public static class DetourHelper {
    private class Loader : ILoadable {
        public void Load(Mod mod) {
        }

        public void Unload() {
            foreach (var hook in loadedHooks) {
                hook.Undo();
            }
            loadedHooks.Clear();
        }
    }

    private static readonly List<Hook> loadedHooks = new();

    public static Hook AddHook(MethodInfo source, MethodInfo target) {
        Hook hook = new(source, target);
        hook.Apply();
        loadedHooks.Add(hook);
        return hook;
    }
}