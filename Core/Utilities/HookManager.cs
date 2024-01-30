using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Aequus.Core.Utilities;

public class HookManager : ILoadable {
    /// <summary><inheritdoc cref="ApplyAndCacheHook(MethodInfo, MethodInfo)"/></summary>
    /// <exception cref="MissingMethodException"></exception>
    public static Hook ApplyAndCacheHook(Type sourceType, Type targetType, String methodName, BindingFlags sourceBindingFlags = BindingFlags.Public | BindingFlags.Static, BindingFlags targetBindingFlags = BindingFlags.NonPublic | BindingFlags.Static) {
        String targetMethodName = $"{sourceType.Name}_{methodName}";
        var sourceMethod = sourceType.GetMethod(methodName, sourceBindingFlags) ?? throw new Exception($"Detour source method \"{targetMethodName}\" was not found.");
        var targetMethod = targetType.GetMethod(targetMethodName, targetBindingFlags);
        return targetMethod == null ? throw new MissingMethodException($"Detour target method \"{targetMethodName}\" not found.") : ApplyAndCacheHook(sourceMethod, targetMethod);
    }

    /// <summary>Applies a hook to a method, and caches it to prevent it from unloading.</summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Hook ApplyAndCacheHook(MethodInfo source, MethodInfo target) {
        Hook hook = new(source, target);
        hook.Apply();
        _loadedHooks.Add(hook);
        return hook;
    }

    private static readonly List<Hook> _loadedHooks = new();

    public void Load(Mod mod) { }

    public void Unload() {
        foreach (var hook in _loadedHooks) {
            hook.Undo();
        }
        _loadedHooks.Clear();
    }
}