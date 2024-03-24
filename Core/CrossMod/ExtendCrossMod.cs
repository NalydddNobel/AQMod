using System;

namespace Aequus.Core.CrossMod;

internal static class ExtendCrossMod {
    public static string GetModFromNamespace(Type type) {
        string typeNamespace = type.Namespace;
        string search = "CrossMod.";
        int index = typeNamespace.LastIndexOf(search);
        if (index == -1) {
            throw new Exception($"Namespace of {type.Name} ({typeNamespace}) does not have a subdirectory of \"CrossMod\".");
        }

        index += search.Length;
        return typeNamespace[index..typeNamespace.IndexOf('.', index)].Replace("Support", "");
    }
}
