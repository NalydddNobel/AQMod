using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Aequus.Common.Utilities.Helpers;

internal static class DetourTool {
    private static readonly List<Hook> _loaded = [];

    public static void Apply(Type baseType, Type hookType, string methodName) {
        string originalMethodName = methodName[(4 + baseType.Name.Length)..];
        MethodInfo? source = baseType.GetMethod(originalMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
        MethodInfo? target = hookType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        Hook hook = new Hook(source!, target!);
        hook.Apply();
        _loaded.Add(hook);
    }
}
