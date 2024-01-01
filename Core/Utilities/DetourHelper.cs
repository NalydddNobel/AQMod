using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;

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

    public static Hook AddHook(Type sourceType, Type targetType, string methodName, BindingFlags sourceBindingFlags = BindingFlags.Public | BindingFlags.Static, BindingFlags targetBindingFlags = BindingFlags.NonPublic | BindingFlags.Static) {
        string targetMethodName = $"{sourceType.Name}_{methodName}";
        var sourceMethod = sourceType.GetMethod(methodName, sourceBindingFlags) ?? throw new Exception($"Detour source method \"{targetMethodName}\" was not found.");
        var targetMethod = targetType.GetMethod(targetMethodName, targetBindingFlags);
        return targetMethod == null ? throw new Exception($"Detour target method \"{targetMethodName}\" not found.") : AddHook(sourceMethod, targetMethod);
    }

    public static Hook AddHook(MethodInfo source, MethodInfo target) {
        Hook hook = new(source, target);
        hook.Apply();
        loadedHooks.Add(hook);
        return hook;
    }
}