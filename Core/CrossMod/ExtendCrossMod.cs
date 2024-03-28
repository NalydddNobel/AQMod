using System;

namespace Aequus.Core.CrossMod;

internal static class ExtendCrossMod {
    private const char FULL_NAME_SEPERATOR = '/';

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

    public static bool GetContentFromName<T>(string fullContentName, out T value) where T : IModType {
        value = default;
        string[] split = fullContentName.Split(FULL_NAME_SEPERATOR);
        if (split.Length != 2) {
            return false;
        }

        string modName = split[0];
        string contentName = split[1];

        if (!ModLoader.TryGetMod(modName, out Mod mod)) {
            return false;
        }

        return mod.TryFind<T>(contentName, out value);
    }
}
